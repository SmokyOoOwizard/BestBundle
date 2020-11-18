using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BestBundle
{
    public class Bundle
    {
        // 0x0F - Header
        // ?    - Bundle Info
        // ?    - Resources Database
        // ?    - Chunks

        private BundleInfo BundleInfo;
        private BundleResourceDatabase resourceDatabase;
    }
}