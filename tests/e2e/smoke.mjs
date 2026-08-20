// Headless browser smoke test for the Blazor-ApexMaps sample.
//
// Why this exists: a Blazor map wrapper can build, and even serialize its options perfectly, and
// still fail in the browser, because the real contract lives in the JS interop and the core
// library. This wrapper loads the core as an ES module, serializes the whole option tree through
// System.Text.Json, compiles caller-supplied formatters from strings, and fetches geometry over the
// network, so a broken import path, a mis-serialized option, an uncompilable formatter or a moved
// pack id would only surface at runtime. This test drives the actual WASM app in a real browser and
// fails if any demo page throws, shows the Blazor error UI, or draws no marks.
//
// The empty-map case is the one worth automating: a page that throws is obvious in a diff, while a
// page that renders nothing because a pack id moved looks fine.
//
// Usage:
//   BASE_URL=http://localhost:5186 node smoke.mjs
//   PW_CHANNEL=chrome node smoke.mjs   # drive an installed Chrome instead of bundled chromium
let chromium;
try {
  ({ chromium } = await import("playwright"));
} catch {
  ({ chromium } = await import("playwright-core"));
}

const BASE = process.env.BASE_URL || "http://localhost:5186";
const channel = process.env.PW_CHANNEL;

// Every route that draws a map, and the mark selector that proves it drew something. Each series
// type paints its own class: areas are `.apexmaps-feature`, bubbles `.apexmaps-bubble`, markers
// `.apexmaps-mark`, arcs `.apexmaps-arc` and given routes `.apexmaps-line`.
const ROUTES = [
  { route: "", marks: ".apexmaps-feature" },
  { route: "choropleth", marks: ".apexmaps-feature" },
  { route: "scales", marks: ".apexmaps-feature" },
  { route: "projections", marks: ".apexmaps-feature" },
  { route: "points", marks: ".apexmaps-bubble, .apexmaps-mark" },
  { route: "routes", marks: ".apexmaps-arc, .apexmaps-line" },
  { route: "drilldown", marks: ".apexmaps-feature" },
  { route: "selection", marks: ".apexmaps-feature" },
  { route: "camera", marks: ".apexmaps-feature" },
  { route: "theming", marks: ".apexmaps-feature" },
  { route: "joins", marks: ".apexmaps-feature" },
  { route: "events", marks: ".apexmaps-feature" },
];

// The sample carries the licence for the public demo, which is domain-locked to
// apexcharts.github.io. Off that domain the core says so, once, and watermarks the licensed
// features: expected noise here rather than a regression, so it is not counted.
const IGNORED_ERRORS = [/License is not valid for this domain/i];

const isRealError = (text) => !IGNORED_ERRORS.some((pattern) => pattern.test(text));

const failures = [];
const browser = await chromium.launch(channel ? { channel, headless: true } : { headless: true });
const page = await browser.newPage({ viewport: { width: 1400, height: 900 } });

let pageErrors = [];
page.on("console", (m) => {
  if (m.type() === "error" && isRealError(m.text())) pageErrors.push(m.text().slice(0, 300));
});
page.on("pageerror", (e) => {
  if (isRealError(e.message)) pageErrors.push("PAGEERROR: " + e.message.slice(0, 300));
});

// Boot once: warms the WASM runtime so the per-route waits are about the map, not the framework.
await page.goto(BASE + "/", { waitUntil: "domcontentloaded" });
await page.waitForSelector("nav, .sidebar, .nav-scrollable", { timeout: 120000 }).catch(() => {});

async function checkRoute({ route, marks }) {
  pageErrors = [];
  await page.goto(`${BASE}/${route}`, { waitUntil: "domcontentloaded" });
  // Geometry is fetched from a CDN on first use, so allow for a network round trip.
  await page.waitForSelector(marks, { timeout: 45000 }).catch(() => {});
  await page.waitForTimeout(1200);

  const state = await page.evaluate((markSelector) => {
    const errorUi = document.querySelector("#blazor-error-ui");
    return {
      errUiShown: errorUi ? getComputedStyle(errorUi).display !== "none" : false,
      root: document.querySelectorAll(".apexmaps").length,
      svg: document.querySelectorAll(".apexmaps-svg").length,
      marks: document.querySelectorAll(markSelector).length,
    };
  }, marks);

  const label = route || "(home)";
  if (pageErrors.length) failures.push(`[${label}] console/page errors: ${JSON.stringify(pageErrors)}`);
  if (state.errUiShown) failures.push(`[${label}] Blazor error UI is visible`);
  if (state.root === 0 || state.marks === 0) {
    failures.push(`[${label}] nothing drawn (root=${state.root}, svg=${state.svg}, marks=${state.marks})`);
  }
  console.log(
    `[${label}] root=${state.root} svg=${state.svg} marks=${state.marks} errUi=${state.errUiShown} errors=${pageErrors.length}`,
  );
}

for (const entry of ROUTES) await checkRoute(entry);

// Interaction: a click on a feature has to reach .NET, which is the whole event bridge in one
// assertion. The events page logs every event it receives.
pageErrors = [];
await page.goto(BASE + "/events", { waitUntil: "domcontentloaded" });
await page.waitForSelector(".apexmaps-feature", { timeout: 45000 });
await page.waitForTimeout(800);
const before = await page.locator(".event-log li").count();
await page.locator(".apexmaps-feature").nth(20).click({ force: true });
await page.waitForTimeout(800);
const after = await page.locator(".event-log li").count();
if (after <= before) {
  failures.push(`[events] a feature click produced no event in .NET (log ${before} -> ${after})`);
}
console.log(`[events] click bridged: log ${before} -> ${after}, errors=${pageErrors.length}`);

// Interaction: the camera API and the export path both cross the boundary in the other direction.
pageErrors = [];
await page.goto(BASE + "/camera", { waitUntil: "domcontentloaded" });
await page.waitForSelector(".apexmaps-feature", { timeout: 45000 });
await page.getByText("Fly to Paris").click();
await page.waitForTimeout(1600);
await page.getByText("Preview as a data URI").click();
await page.waitForSelector("img.export-preview", { timeout: 20000 }).catch(() => {});
const exported = await page.evaluate(() => {
  const img = document.querySelector("img.export-preview");
  return img ? img.getAttribute("src")?.startsWith("data:image/png") : false;
});
if (!exported) failures.push("[camera] the PNG export produced no data URI");
if (pageErrors.length) failures.push(`[camera] console/page errors: ${JSON.stringify(pageErrors)}`);
console.log(`[camera] export=${exported} errors=${pageErrors.length}`);

// Interaction: drilldown swaps the geometry for a deeper pack, which is a second network fetch and
// a re-render, and reports back through the drilldown event.
pageErrors = [];
await page.goto(BASE + "/drilldown", { waitUntil: "domcontentloaded" });
await page.waitForSelector(".apexmaps-feature", { timeout: 45000 });
await page.getByText("Drill to California").click();
await page.waitForTimeout(3000);
const drilled = await page.evaluate(() => ({
  breadcrumb: document.querySelectorAll(".apexmaps-breadcrumb-item, .apexmaps-breadcrumb-current").length,
  log: document.querySelectorAll(".event-log li").length,
  features: document.querySelectorAll(".apexmaps-feature").length,
}));
if (drilled.log === 0) failures.push("[drilldown] the drilldown event never reached .NET");
console.log(
  `[drilldown] breadcrumb=${drilled.breadcrumb} log=${drilled.log} features=${drilled.features} errors=${pageErrors.length}`,
);

await browser.close();

if (failures.length) {
  console.error("\nE2E SMOKE FAILED:\n" + failures.map((f) => "  - " + f).join("\n"));
  process.exit(1);
}
console.log("\nE2E smoke passed: every demo page drew a map, and events, the camera, export and drilldown all crossed the interop boundary.");
