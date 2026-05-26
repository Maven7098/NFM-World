namespace NFMWorld;

public class CarMesh(GraphicsDevice graphicsDevice, Rad3d rad) : Mesh(graphicsDevice, rad)
{
    public CarStats Stats = CarStats.ValidateStats(rad.Stats, rad.FileName);
    public Rad3dWheelDef[] Wheels = rad.Wheels;
    public Rad3dRimsDef? Rims = rad.Rims;
}