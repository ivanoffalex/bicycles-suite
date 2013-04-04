namespace BicyclesSuite.Shared.Reflection
{
    /// <summary>
    /// Interface for creation instance by prototype 
    /// </summary>
    public interface IPrototypeInstance
    {
        /// <summary>
        /// Create instance of object
        /// </summary>
        /// <returns>New instance by prototype</returns>
        IPrototypeInstance CreateInstance();
    }

}
