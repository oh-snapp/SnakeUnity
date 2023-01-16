# SnakeUnity

A simple snake implementation for unity.

![Demo](https://user-images.githubusercontent.com/29184562/212602059-cb053062-c07e-42fd-8fc1-61f437eea0a3.gif)

The actual movement code is in `Assets/Snek.cs` and is about 130 lines of code.

Right now, the implementation uses a `List` instead of a `Stack` for coordinates (as the back element is necessary for movement), but this can be optimized by simply storing the back element coordinate separately.
