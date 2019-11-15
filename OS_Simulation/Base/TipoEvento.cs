namespace OS_Simulation
{
    public enum TipoEvento
    {
        ARRIVAL,                        // 1
        REQUEST_CM,                     // 2
        REQUEST_CPU,                    // 3
        RELEASE_CPU_REQUEST_DISK,       // 4
        REQUEST_DISK,                   // 5
        RELEASE_DISK,                   // 6
        RELEASE_CM_CPU,                 // 7
        COMPLETION                      // 8
    };
}