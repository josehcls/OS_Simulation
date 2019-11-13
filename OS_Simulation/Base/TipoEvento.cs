namespace OS_Simulation
{
    public enum TipoEvento
    {
        ARRIVAL,        // 1
        REQUEST_CM,     // 2
        REQUEST_CPU,    // 3
        REQUEST_DISK,   // 4
        RELEASE_DISK,   // 5
        RELEASE_CM_CPU, // 6
        COMPLETION      // 7
    };
}