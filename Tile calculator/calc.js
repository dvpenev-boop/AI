// calc.js - pure calculation utilities for tile counts

export function mmToM(mm){
  return mm/1000;
}

export function area(mmWidth, mmHeight){
  // returns m^2
  return mmToM(mmWidth) * mmToM(mmHeight);
}

export function ceilTilesForArea(areaM2, tileWidthMm, tileHeightMm){
  // compute how many whole tiles required to cover areaM2, without considering layout waste
  const tileArea = area(tileWidthMm, tileHeightMm);
  if (tileArea <= 0) return 0;
  return Math.ceil(areaM2 / tileArea);
}

export function calculateFloorTiles(floorWmm, floorLmm, tileWmm, tileLmm){
  const floorArea = area(floorWmm, floorLmm);
  const raw = ceilTilesForArea(floorArea, tileWmm, tileLmm);
  return {floorArea, raw};
}

export function calculateWallTiles(floorWmm, floorLmm, wallHmm, tileWmm, tileHmm, doorWmm = 0, doorHmm = 0){
  // We model walls as four walls around the floor's perimeter: two walls with length floorW, two with floorL
  // total wall area = perimeter * height - door area
  const perimeter = 2 * (floorWmm + floorLmm);
  const wallArea = area(perimeter, wallHmm); // mm->m conversion inside area
  const doorArea = area(doorWmm, doorHmm);
  const netWallArea = Math.max(0, wallArea - doorArea);
  const raw = ceilTilesForArea(netWallArea, tileWmm, tileHmm);
  return {perimeter, wallArea, doorArea, netWallArea, raw};
}

export function applyWaste(countRaw, wastePercent){
  const multiplier = 1 + Math.max(0, wastePercent)/100;
  return Math.ceil(countRaw * multiplier);
}
