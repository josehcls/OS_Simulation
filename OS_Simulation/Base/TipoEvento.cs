namespace OS_Simulation
{
    public enum TipoEvento
    {
        ARRIVAL,
        REQUEST_CM,
        REQUEST_CPU,
        REQUEST_DISK,
        RELEASE_DISK,
        RELEASE_CM_CPU,
        COMPLETION
    };
}