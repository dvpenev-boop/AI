# Bathroom Tile Calculator

Simple browser app to calculate floor and wall tile requirements from millimetre measurements and visualize the layouts.

Features:
- Enter floor dimensions and floor tile size (mm)
- Enter wall height and wall tile size (mm)
- Enter door size to subtract door area from wall tiling
- Waste allowance percentage (default 10%)
- Visual previews for floor and walls (SVG)

Files:
- `index.html` — main UI
- `styles.css` — styling
- `main.js` — UI wiring
- `calc.js` — tile/area calculations (module)
- `renderer.js` — simple SVG renderers (module)

How to use:
1. Open `index.html` in a modern browser (Chrome, Edge, Firefox).
2. Adjust values in the form and press Calculate to see counts and previews.

Notes & assumptions:
- All inputs are millimetres.
- Counts are rounded up to whole tiles.
- Waste is applied as a percentage on raw counts.
- Wall rendering is an "unwrapped" representation (perimeter as a strip).

Possible improvements:
- Support multiple doors/windows on walls
- More realistic layout logic minimizing cuts
- Export printable material list

License: MIT
