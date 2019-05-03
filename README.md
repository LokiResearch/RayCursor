# RayCursor: A 3D Pointing Facilitation Technique based on Raycasting

This is a Unity Project containing the source code and prefab for the pointing technique RayCursor
presented in [this paper](https://dx.doi.org/10.1145/3290605.3300331).

## Usage in your Unity project

1. Copy the folder `Assets/RayCursor` in your assets directory.
2. In your scene, put the prefab `RayCursor` as a child of the GameObject reprenting a VR controller.
3. On each GameObject you want to be selectable using RayCursor, add the component `Selectable`.
   These objects requires a Collider and a MeshRenderer.
   The highlight of the closest target only works on objects which have exactly one Material in the MeshRenderer.
   Each `Selectable` instance has an `OnSelect` event that you can listen to via scripting.
4. Configure the inputs. The configuration depend on the API you use to access the controller inputs. See the next section for more details.

- Minimum version of Unity: `2018.3.0f2`
- Only one instance of `RayCursor` is possible in a scene.

## Input configuration

### Legacy input system of unity (`Input.getXXX()`)

You have to configure 3 inputs in the configuration screen `Edit -> Project Settings... -> Input`

- `RayCursorSelect`: this input is used to trigger the selection. This is usually mapped to the trigger button of the controller.
- `RayCursorTouch`: this input is used to detect the presence of the finger on the touchpad.
- `RayCursorPadY`: this input is used to detect the `y` position of the finger on the touchpad.

You can copy the configuration of the inputs from the configuration of this project for the HTC Vive (Left Controller).

### Others API (Oculus, OpenVR, ...)

You have to change the source code of the 3 methods in `Scripts/RayCursor.cs`:

- `bool RayCursor.GetInputSelect()`
- `bool RayCursor.GetInputTouch()`
- `float RayCursor.GetInputTouchY()`

## License

This project is published under the MIT License (see `LICENSE.md`).
