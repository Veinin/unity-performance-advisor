using System;

namespace UPA
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RuleAttribute : Attribute
    {
        public AssetType assetType;
        
        public RuleAttribute(AssetType type) {
            assetType = type;
        }
    }
}