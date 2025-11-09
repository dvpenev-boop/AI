// renderer.js - provides simple SVG rendering for floor and walls

// Draw floor plan to svgEl (a square area scaled); simple top-down grid of tiles
export function renderFloor(svgEl, params){
  // params: floorWmm, floorLmm, tileWmm, tileLmm
  const {floorWmm, floorLmm, tileWmm, tileLmm} = params;
  clearSvg(svgEl);
  const vbW = 1000, vbH = 600; // viewBox
  svgEl.setAttribute('viewBox', `0 0 ${vbW} ${vbH}`);

  // scale mm to viewbox units maintaining floor ratio
  const scale = Math.min(vbW / floorWmm, vbH / floorLmm);
  const drawW = floorWmm * scale;
  const drawH = floorLmm * scale;
  const ox = (vbW - drawW)/2;
  const oy = (vbH - drawH)/2;

  // background rect
  appendRect(svgEl, ox, oy, drawW, drawH, '#f8fafc', '#e6eefc');

  // tile sizes in view units
  const tw = tileWmm * scale;
  const th = tileLmm * scale;

  // draw grid of tiles, stepping across width (x) and length (y)
  for(let x = 0; x < drawW; x += tw){
    for(let y = 0; y < drawH; y += th){
      const w = Math.min(tw, drawW - x);
      const h = Math.min(th, drawH - y);
      const rx = Math.round(ox + x);
      const ry = Math.round(oy + y);
      appendRect(svgEl, rx, ry, w, h, 'transparent', 'rgba(15,23,42,0.04)');
    }
  }

  // outline
  appendRect(svgEl, ox, oy, drawW, drawH, 'transparent', '#111827', 2, 'none');
}

// Draw walls in a single unwrapped strip; subtract door rectangle if present
export function renderWalls(svgEl, params){
  // params: floorWmm, floorLmm, wallHmm, tileWmm, tileHmm, doorWmm, doorHmm
  const {floorWmm, floorLmm, wallHmm, tileWmm, tileHmm, doorWmm, doorHmm} = params;
  clearSvg(svgEl);
  const vbW = 1000, vbH = 600;
  svgEl.setAttribute('viewBox', `0 0 ${vbW} ${vbH}`);

  // unwrapped length is perimeter
  const perim = 2 * (floorWmm + floorLmm);
  const scale = Math.min(vbW / perim, vbH / wallHmm);
  const drawW = perim * scale;
  const drawH = wallHmm * scale;
  const ox = (vbW - drawW)/2;
  const oy = (vbH - drawH)/2;

  appendRect(svgEl, ox, oy, drawW, drawH, '#fffaf0', '#fef3c7');

  const tw = tileWmm * scale;
  const th = tileHmm * scale;

  for(let x = 0; x < drawW; x += tw){
    for(let y = 0; y < drawH; y += th){
      const w = Math.min(tw, drawW - x);
      const h = Math.min(th, drawH - y);
      const rx = Math.round(ox + x);
      const ry = Math.round(oy + y);
      appendRect(svgEl, rx, ry, w, h, 'transparent', 'rgba(15,23,42,0.04)');
    }
  }

  // door: find position along unwrapped wall: place it at start (for simplicity), draw subtraction
  if(doorWmm > 0 && doorHmm > 0){
    const dw = doorWmm * scale;
    const dh = doorHmm * scale;
    // center door on first wall segment of length floorWmm
    const doorX = ox + (floorWmm*scale - dw)/2;
    const doorY = oy + drawH - dh; // bottom aligned
    appendRect(svgEl, doorX, doorY, dw, dh, '#fff', 'rgba(0,0,0,0.06)');
    appendText(svgEl, doorX + 5, doorY + 16, 'Door', 12, '#374151');
  }

  appendRect(svgEl, ox, oy, drawW, drawH, 'transparent', '#111827', 2, 'none');
}

function clearSvg(svgEl){
  while(svgEl.firstChild) svgEl.removeChild(svgEl.firstChild);
}

function appendRect(svgEl, x, y, w, h, fill='transparent', stroke='rgba(0,0,0,0.06)', strokeWidth=1, strokeDash=''){ 
  const ns = 'http://www.w3.org/2000/svg';
  const r = document.createElementNS(ns, 'rect');
  r.setAttribute('x', x);
  r.setAttribute('y', y);
  r.setAttribute('width', w);
  r.setAttribute('height', h);
  r.setAttribute('fill', fill);
  if(stroke) r.setAttribute('stroke', stroke);
  if(strokeWidth) r.setAttribute('stroke-width', strokeWidth);
  if(strokeDash) r.setAttribute('stroke-dasharray', strokeDash);
  svgEl.appendChild(r);
}

function appendText(svgEl, x, y, text, size=14, color='#000'){
  const ns = 'http://www.w3.org/2000/svg';
  const t = document.createElementNS(ns, 'text');
  t.setAttribute('x', x);
  t.setAttribute('y', y);
  t.setAttribute('fill', color);
  t.setAttribute('font-size', size);
  t.setAttribute('font-family', 'Inter, Arial, sans-serif');
  t.textContent = text;
  svgEl.appendChild(t);
}
