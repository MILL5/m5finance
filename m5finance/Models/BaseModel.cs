using HashCode = Pineapple.Common.HashCode;

namespace M5Finance.Models
{
    public abstract class BaseModel
    {
        protected internal abstract void ComputeHash(HashCode hash);
    }
}
