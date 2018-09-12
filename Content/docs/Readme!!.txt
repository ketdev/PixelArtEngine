Exporting stuff from blender:

- You have to APPLY (Ctrl + A) all transformations in blender before exporting
- Remove the texture, load manually. Android doesn't detect automatically
- Animation speed is at about 30 fps when exported

Mesh:
 - Remove duplicate vertices
 - Simplify faces (beautify), careful that it doesn't break animation seams

FBX Export: 
 - Y is Forward Z is Up
 - Export scale 0.01 for bones

Texture:
 - 128x128 is enough for 1 block objects
 - Smart UV unwrap, with margin of 0.1, make sure nothing uses topleft pixel
 - Setup 36x36 area lamp at z36 with distance 36
 - bake texture into black-white shades
 - Use a gradient map in photoshop for coloring
 - Set topleft pixel in border color
 - Split UV areas with different colors so it is easier to color in photoshop