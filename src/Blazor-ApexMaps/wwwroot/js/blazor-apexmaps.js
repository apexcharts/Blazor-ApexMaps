/*! Blazor-ApexMaps interop bridge (ES module) */
//
// Self-contained: imports the vendored apexmaps ESM build directly, so the host app needs no
// script tag and no CDN reference for the core library. The core injects its own stylesheet, so
// there is no CSS link to add either.
import ApexMaps from "./apexmaps.esm.js?ver=0.3.0";

// map instances and their event-listener cleanups, keyed by element id
const instances = {};
const cleanups = {};

// core event name -> Blazor [JSInvokable] handler method. The core emits through its own
// emitter (map.on) rather than DOM events, so these are subscriptions, not listeners.
const EVENT_HANDLERS = {
  rendered: "HandleRendered",
  updated: "HandleUpdated",
  resized: "HandleResized",
  featureClick: "HandleFeatureClick",
  featureHover: "HandleFeatureHover",
  featureFocus: "HandleFeatureFocus",
  markClick: "HandleMarkClick",
  markHover: "HandleMarkHover",
  clusterClick: "HandleClusterClick",
  drilldown: "HandleDrilldown",
  drillup: "HandleDrillup",
  selectionChange: "HandleSelectionChange",
  legendToggle: "HandleLegendToggle",
  zoom: "HandleZoom",
  panEnd: "HandlePanEnd",
  rotate: "HandleRotate",
  rotateEnd: "HandleRotateEnd",
};

// Events that carry no payload at all, so .NET gets no argument rather than an empty object.
const VOID_EVENTS = new Set(["panEnd"]);

// The marker JsFunction serializes to. Any object of this exact shape, anywhere in the options
// tree, is a function the caller wrote in C# as source.
const FN_MARKER = "__apexJsFn";

/**
 * Revive `{ "__apexJsFn": "<source>" }` markers into real functions, in place.
 *
 * Compiled with `new Function` and therefore evaluated in the global scope: a formatter can see
 * `window` and cannot see the caller's C# state, which is the same contract the other framework
 * wrappers give a formatter written in the host language.
 */
function reviveFunctions(value) {
  if (value === null || typeof value !== "object") return value;

  if (Array.isArray(value)) {
    for (let i = 0; i < value.length; i++) value[i] = reviveFunctions(value[i]);
    return value;
  }

  const keys = Object.keys(value);
  if (keys.length === 1 && keys[0] === FN_MARKER && typeof value[FN_MARKER] === "string") {
    const source = value[FN_MARKER];
    try {
      // Wrapped in a return so both `x => ...` and `function (x) { ... }` are expressions.
      return new Function(`"use strict"; return (${source});`)();
    } catch (error) {
      console.error("apexmaps: could not compile a JsFunction:", source, error);
      return undefined;
    }
  }

  for (const key of keys) value[key] = reviveFunctions(value[key]);
  return value;
}

function parseOptions(optionsJson) {
  return reviveFunctions(JSON.parse(optionsJson || "{}"));
}

/**
 * Strip what cannot cross the interop boundary from an event payload.
 *
 * Every core payload carries `instance`, the map object itself: it is there so a JavaScript
 * handler can act on the map without a closure, and it is circular, so serializing it would
 * throw. .NET has the component for that, so it goes.
 */
function toPayload(detail) {
  if (detail === null || detail === undefined) return null;
  const { instance, ...rest } = detail;
  return rest;
}

function mapOf(elementId) {
  const map = instances[elementId];
  if (!map) throw new Error(`ApexMaps instance not found: ${elementId}`);
  return map;
}

/** Present but not rendered yet is a normal state during teardown, so callers can ask. */
export function exists(elementId) {
  return Boolean(instances[elementId]);
}

export function setLicense(licenseKey) {
  try {
    if (typeof ApexMaps.setLicense === "function") {
      ApexMaps.setLicense(licenseKey);
      return true;
    }
    console.error("ApexMaps.setLicense is not available");
    return false;
  } catch (error) {
    console.error("failed to set apexmaps license:", error);
    return false;
  }
}

/**
 * Point the geometry registry somewhere else: a self-hosted copy of `apexmaps-geo`, an
 * air-gapped path, or your own CDN. Packs are fetched from here, one request per pack.
 */
export function setGeoSource(source) {
  try {
    ApexMaps.setGeoSource(source);
    return true;
  } catch (error) {
    console.error("failed to set the apexmaps geo source:", error);
    return false;
  }
}

/** Register geometry under an id, so `geo.map` can name it like a built-in pack. */
export function registerMap(id, geometryJson, metaJson) {
  try {
    ApexMaps.registerMap(id, JSON.parse(geometryJson), metaJson ? JSON.parse(metaJson) : undefined);
    return true;
  } catch (error) {
    console.error("failed to register a map:", error);
    return false;
  }
}

/** Register a named palette: `{ kind: 'sequential' | 'diverging' | 'categorical', stops: [...] }`. */
export function registerPalette(name, paletteJson) {
  try {
    ApexMaps.registerPalette(name, JSON.parse(paletteJson));
    return true;
  } catch (error) {
    console.error("failed to register a palette:", error);
    return false;
  }
}

export function listMaps() {
  return ApexMaps.listMaps();
}

export function listProjections() {
  return ApexMaps.listProjections();
}

export function listPalettes() {
  return ApexMaps.listPalettes();
}

export function mapMeta(id) {
  const meta = ApexMaps.mapMeta(id);
  return meta ? JSON.parse(JSON.stringify(meta)) : null;
}

export function version() {
  return ApexMaps.version;
}

export async function init(elementId, optionsJson, dotNetRef) {
  try {
    const element = document.getElementById(elementId);
    if (!element) {
      console.error("element not found:", elementId);
      return false;
    }

    const map = new ApexMaps(element, parseOptions(optionsJson));
    instances[elementId] = map;

    if (dotNetRef) {
      const added = [];
      for (const [eventName, handler] of Object.entries(EVENT_HANDLERS)) {
        const listener = (payload) => {
          // Fire and forget: a handler that throws in .NET must not break the core's emit loop,
          // and a disposed circuit rejects rather than throwing synchronously.
          const args = VOID_EVENTS.has(eventName) ? [] : [toPayload(payload)];
          dotNetRef.invokeMethodAsync(handler, ...args).catch(() => {});
        };
        map.on(eventName, listener);
        added.push([eventName, listener]);
      }
      cleanups[elementId] = () => added.forEach(([name, listener]) => map.off(name, listener));
    }

    // Geometry is fetched here, so this is the await that matters: a registry pack is one
    // request, and the map is not on screen until it lands.
    await map.render();
    return true;
  } catch (error) {
    console.error("failed to initialize apexmaps:", error);
    delete instances[elementId];
    return false;
  }
}

/** Merge options and redraw. Geometry is reprojected when the map or the projection changed. */
export async function updateOptions(elementId, optionsJson, redrawGeometry) {
  await mapOf(elementId).updateOptions(parseOptions(optionsJson), {
    redrawGeometry: redrawGeometry || undefined,
  });
}

/** Replace the series, tweening fills and radii rather than rebuilding the DOM. */
export function updateSeries(elementId, seriesJson) {
  mapOf(elementId).updateSeries(parseOptions(seriesJson));
}

export function destroy(elementId) {
  try {
    cleanups[elementId]?.();
    delete cleanups[elementId];
    const map = instances[elementId];
    if (map) {
      map.destroy();
      delete instances[elementId];
    }
    return true;
  } catch (error) {
    console.error("failed to destroy the map:", error);
    return false;
  }
}

// --- drilldown ---------------------------------------------------------------
export function drillTo(elementId, key) {
  return mapOf(elementId).drillTo(key);
}
export function drillUp(elementId, levels) {
  return mapOf(elementId).drillUp(levels === null || levels === undefined ? 1 : levels);
}
export function drillDepth(elementId) {
  return mapOf(elementId).drillDepth;
}

// --- selection ---------------------------------------------------------------
export function toggleSelection(elementId, key) {
  mapOf(elementId).toggleSelection(key);
}
export function setSelection(elementId, keysJson) {
  mapOf(elementId).setSelection(JSON.parse(keysJson));
}
export function clearSelection(elementId) {
  mapOf(elementId).clearSelection();
}

// --- camera ------------------------------------------------------------------
export function flyTo(elementId, targetJson) {
  return mapOf(elementId).camera.flyTo(JSON.parse(targetJson));
}
export function easeTo(elementId, targetJson) {
  return mapOf(elementId).camera.easeTo(JSON.parse(targetJson));
}
export function jumpTo(elementId, targetJson) {
  mapOf(elementId).camera.jumpTo(JSON.parse(targetJson));
}
export function frameFeature(elementId, key, optionsJson) {
  return mapOf(elementId).frameFeature(key, optionsJson ? JSON.parse(optionsJson) : {});
}
export function resetView(elementId, optionsJson) {
  return mapOf(elementId).resetView(optionsJson ? JSON.parse(optionsJson) : {});
}
export function zoomIn(elementId) {
  mapOf(elementId).zoomIn();
}
export function zoomOut(elementId) {
  mapOf(elementId).zoomOut();
}
export function getZoom(elementId) {
  return mapOf(elementId).zoom;
}
export function rotateTo(elementId, anglesJson) {
  mapOf(elementId).rotateTo(JSON.parse(anglesJson));
}
export function getRotation(elementId) {
  return mapOf(elementId).rotation;
}

// --- diagnostics and export --------------------------------------------------

/**
 * Flatten the join report into something JSON can carry: the core's own result holds Maps and a
 * `report()` function, which .NET cannot receive.
 */
export function diagnoseJoin(elementId, seriesIndex) {
  const result = mapOf(elementId).diagnoseJoin(seriesIndex || 0);
  if (!result) return null;
  return {
    matched: result.matched,
    totalData: result.totalData,
    totalFeatures: result.totalFeatures,
    geoKeyField: result.geoKeyField,
    dataKeyField: result.dataKeyField,
    unmatchedData: (result.unmatchedData ?? []).map((row) => ({
      key: row.key,
      suggestions: row.suggestions ?? [],
    })),
    unmatchedFeatures: result.unmatchedFeatures ?? [],
    sharedKeys: result.sharedKeys ?? [],
    applied: result.applied ?? [],
    report: typeof result.report === "function" ? result.report() : null,
  };
}

/** The resolved options tree, with functions dropped: JSON in, JSON out. */
export function toSpec(elementId) {
  return JSON.stringify(mapOf(elementId).toSpec());
}

export function getSvgString(elementId, optionsJson) {
  return mapOf(elementId).getSvgString(optionsJson ? JSON.parse(optionsJson) : {});
}
export function exportSVG(elementId, optionsJson) {
  mapOf(elementId).exportSVG(optionsJson ? JSON.parse(optionsJson) : {});
}
export function exportPNG(elementId, optionsJson) {
  return mapOf(elementId).exportPNG(optionsJson ? JSON.parse(optionsJson) : {});
}
export async function dataURI(elementId, optionsJson) {
  const result = await mapOf(elementId).dataURI(optionsJson ? JSON.parse(optionsJson) : {});
  return result.imgURI;
}
