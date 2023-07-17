# OpenTK Sample

This is a sample project demonstrating the usage of OpenTK library for graphics programming. It includes a simple program that renders a 2D image with shadow effects.

## Getting Started

To run this sample, you will need to have the OpenTK library installed. You can download it from the official OpenTK website: [https://opentk.net/](https://opentk.net/)

## Installation

1. Clone or download the project from the repository.
2. Open the project in Visual Studio.
3. Make sure you have the OpenTK library referenced in your project. If not, you can add it by following these steps:
   - Right-click on your project in the Solution Explorer.
   - Select "Manage NuGet Packages."
   - Search for "OpenTK" and install the package.
4. Build the project to ensure all dependencies are resolved.

## Usage

The sample project consists of a `Program_shadow` class that handles the rendering of the 2D image with shadow effects. You can use the class in your own application by following these steps:

1. Open your existing or new project in Visual Studio.
2. Add the necessary `using` statements at the beginning of your code:

```csharp
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
```

3. Copy the `Program_shadow` class from the sample project into your project.
4. Create an instance of the `Program_shadow` class:

```csharp
Program_shadow program = new Program_shadow();
```

5. Initialize the 2D image texture by calling the `init_2D_image_texture` method:

```csharp
program.init_2D_image_texture();
```

6. Initialize the car shadow by calling the `init_carShadow` method:

```csharp
program.init_carShadow();
```

7. Initialize the program by calling the `init_program` method:

```csharp
program.init_program();
```

8. Set the image ID and window mode by calling the `SettingImgId` method. The image ID corresponds to the index of the image you want to display (0 for "original" and 1 for "hdr"). The window mode determines whether to apply shadow effects (0 for no effects and 1 for effects):

```csharp
int imageId = 0; // Set the desired image ID
int windowMode = 1; // Set the desired window mode
program.SettingImgId(imageId, windowMode);
```

9. Render the shadow by calling the `draw_Shadow` method:

```csharp
program.draw_Shadow();
```

10. Run your project to see the rendered 2D image with shadow effects.

## Customization

You can customize various parameters and shaders in the `Program_shadow` class to achieve different effects. Here are some of the properties you can modify:

- `exposure`: Controls the exposure level of the image.
- `gamma`: Controls the gamma correction applied to the image.
- `weight`: Controls the weight used in the shadow effect calculation.
- `nmax` and `cmax`: Control the maximum values used in the shadow effect calculation.

You can modify these properties according to your requirements and experiment with different values to achieve the desired visual effect.

## License

This sample project is licensed under the [MIT License](LICENSE). Feel free to modify and use it in your own projects.

## Credits

This sample project utilizes the OpenTK library, which is an open-source project. You can find more information about OpenTK on the official website: [https://opentk.net/](https://opentk.net/)
