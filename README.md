Threedea will allow you to create 3D concept maps in virtual reality. The goal is to enhance the idea of 2D mind maps, giving the option to create more expressive non-hierarchical maps.

This project uses Unity's [EditorVR](https://github.com/Unity-Technologies/EditorVR) as a platform and is targeted at game developers. This concept can be particularly useful when creating the "concept map" of scenes already used by the developers.

# Development

Threedea development is **ON HOLD**, waiting for further development and stabilization of EditorVR.

Regardless of the development stage, always keep your project under source control to avoid data loss from the tool.

# Usage

This repository is designed to be cloned/extracted inside the "Assets" folder of your project.

# Requirements

- [EditorVR](https://github.com/Unity-Technologies/EditorVR) enabled project - see Unity's instructions to set it up. Keep in mind that this requires a compatible room scale VR headset - currently, Vive or Oculus Touch.
- Powerful hardware or tolerance to VR-sickness: without optimization, complex maps can make it hard to maintain 90hz.

# Features
Some desired features

- [ ] Core
  - [ ] Threedea tool
  - [ ] Threedea workspace
- [ ] Idea (each topic, note or concept)
  - [ ] Creation
    - [ ] On GameObjects
    - [ ] On links
    - [ ] Free positioning
  - [ ] Customization
    - [ ] Direction
    - [ ] Style
- [ ] Notes
  - [ ] Texture / Sprite based
  - [ ] 2D
    - [ ] Basic note drawing on a plane
    - [ ] Note manipulation
    - [ ] Visualization in the standard inspector
  - [ ] 3D
    - [ ] World space notes
    - [ ] Local space notes
    - [ ] Embedding of 2D notes
