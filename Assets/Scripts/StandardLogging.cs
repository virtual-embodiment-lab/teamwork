namespace StandardLogging
{
    [System.Flags]
    public enum logtype
    {
        //Position = 1,
        //Position_X = 2,
        //Position_Y = 4,
        //Position_Z = 8,
        //Rotation = 16,
        //Rotation_X = 32,
        //Rotation_Y = 64,
        //Rotation_Z = 128,
        //Scale = 256,
        //Scale_X = 512,
        //Scale_Y = 1024,
        //Scale_Z = 2048,
        //State = 4096,
        Position = 1,
        Rotation = 2,
        Scale = 4,
        State = 8,
        Vision = 16,
        VRRig = 32,
        Gaze = 64,
    }

    public enum logmethod
    {
        discrete,
        continuous,
    }
}
