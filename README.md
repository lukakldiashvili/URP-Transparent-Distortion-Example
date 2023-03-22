### Distortion effect for Unity URP (supports transparent objects)

<img width="892" alt="image" src="https://user-images.githubusercontent.com/42884387/226945386-10d4c31f-9150-4598-a1dc-e41b69795d7b.png">

### Usage
- Clone repository and open in Unity (2020 or later, but may work with 2019)
- Make sure you have a separate layer created for objects that should display distortion (you can name it "Distortion")
- DO NOT forget to applt that layer to every object or particle effect that uses distortion material
- Check URP render data asset and setup your project corretly (reference below if you dont want to open project)
<img width="484" alt="image" src="https://user-images.githubusercontent.com/42884387/226942924-7ad2ffc3-1d16-46d8-a00f-4d8855066040.png">
- I recommend copying settings as shown, otherwise effect might not work
- Apply material and layer to object that should display the distortion effect
- Effect should now be working!

<br>

### Additional Notes:
- Sample shader is included, or you can build your own shader graph or shader code ones based on it
- Effect uses global texture as instead of SceneColor node, so you can access it in any ways you desire


### Credits
Shader effect is based on GabrielAguiar's - https://www.youtube.com/watch?v=CXCyVDEplyM
