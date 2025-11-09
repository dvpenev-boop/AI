import { calculateFloorTiles, calculateWallTiles, applyWaste } from './calc.js';
import { renderFloor, renderWalls } from './renderer.js';

// Wire UI
const form = document.getElementById('calc-form');
const floorRawEl = document.getElementById('floor-raw');
const floorWasteEl = document.getElementById('floor-waste');
const wallRawEl = document.getElementById('wall-raw');
const wallWasteEl = document.getElementById('wall-waste');
const totalTilesEl = document.getElementById('total-tiles');
const vizFloor = document.getElementById('viz-floor');
const vizWalls = document.getElementById('viz-walls');

form.addEventListener('submit', (e) => {
  e.preventDefault();
  const vals = readInputs();
  if(!vals) return;

  const floor = calculateFloorTiles(vals.floorWidth, vals.floorLength, vals.floorTileW, vals.floorTileL);
  const walls = calculateWallTiles(vals.floorWidth, vals.floorLength, vals.wallHeight, vals.wallTileW, vals.wallTileH, vals.doorW, vals.doorH);

  const floorWithWaste = applyWaste(floor.raw, vals.waste);
  const wallWithWaste = applyWaste(walls.raw, vals.waste);
  const total = floorWithWaste + wallWithWaste;

  floorRawEl.textContent = floor.raw.toLocaleString();
  floorWasteEl.textContent = floorWithWaste.toLocaleString();
  wallRawEl.textContent = walls.raw.toLocaleString();
  wallWasteEl.textContent = wallWithWaste.toLocaleString();
  totalTilesEl.textContent = total.toLocaleString();

  // Render visual previews
  renderFloor(vizFloor, {floorWmm: vals.floorWidth, floorLmm: vals.floorLength, tileWmm: vals.floorTileW, tileLmm: vals.floorTileL});
  renderWalls(vizWalls, {floorWmm: vals.floorWidth, floorLmm: vals.floorLength, wallHmm: vals.wallHeight, tileWmm: vals.wallTileW, tileHmm: vals.wallTileH, doorWmm: vals.doorW, doorHmm: vals.doorH});
});

form.addEventListener('reset', () => {
  setTimeout(() => {
    floorRawEl.textContent = '—';
    floorWasteEl.textContent = '—';
    wallRawEl.textContent = '—';
    wallWasteEl.textContent = '—';
    totalTilesEl.textContent = '—';
    // clear visuals
    if(vizFloor) while(vizFloor.firstChild) vizFloor.removeChild(vizFloor.firstChild);
    if(vizWalls) while(vizWalls.firstChild) vizWalls.removeChild(vizWalls.firstChild);
  }, 30);
});

function readInputs(){
  const floorWidth = Number(document.getElementById('floor-width').value);
  const floorLength = Number(document.getElementById('floor-length').value);
  const floorTileW = Number(document.getElementById('floor-tile-w').value);
  const floorTileL = Number(document.getElementById('floor-tile-l').value);
  const wallHeight = Number(document.getElementById('wall-height').value);
  const wallTileW = Number(document.getElementById('wall-tile-w').value);
  const wallTileH = Number(document.getElementById('wall-tile-h').value);
  const doorW = Number(document.getElementById('door-w').value) || 0;
  const doorH = Number(document.getElementById('door-h').value) || 0;
  const waste = Number(document.getElementById('waste').value) || 0;

  // Basic validation
  if([floorWidth, floorLength, floorTileW, floorTileL, wallHeight, wallTileW, wallTileH].some(v => !isFinite(v) || v <= 0)){
    alert('Please provide positive numeric values for dimensions and tile sizes.');
    return null;
  }

  return {floorWidth, floorLength, floorTileW, floorTileL, wallHeight, wallTileW, wallTileH, doorW, doorH, waste};
}

// auto-calc on load with default values
window.addEventListener('DOMContentLoaded', () => {
  form.dispatchEvent(new Event('submit'));
});
