using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimSol
{
    internal class CompProperties_SolusDrill : CompProperties
    {
        public CompProperties_SolusDrill()
        {
            this.compClass = typeof(CompSolusDrill);
        }

        public int radius = 2;
        public float miningSpeed = 0.5f;

        public int MiningProgressPerTickRare { get => Mathf.RoundToInt(miningSpeed*416); }
    }
}
