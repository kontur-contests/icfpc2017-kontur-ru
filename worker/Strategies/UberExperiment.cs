using lib.Ai;
using lib.Ai.StrategicFizzBuzz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace worker.Strategies
{
    public class UberExperiment : IExperiment
    {

        IAi CreateBot(string className, Dictionary<string, double> pars)
        {
            switch (className)
            {
                case nameof(LochKillerAi): return new LochKillerAi();
                case nameof(ConnectClosestMinesAi): return new ConnectClosestMinesAi();
                case nameof(LochMaxVertexWeighterKillerAi): return new LochMaxVertexWeighterKillerAi();
                case nameof(MaxReachableVertexWeightAi): return new MaxReachableVertexWeightAi(pars["param1"]);
                case nameof(Podnaserator2000Ai): return new Podnaserator2000Ai((int)pars["param1"], (int)pars["param2"], (int)pars["param3"]);
                case nameof(FutureIsNow): return new FutureIsNow(pars["param1"]);
                    
                default: throw new Exception("Unknown classname " + className);
            }
        }

        public Result Play(Task task)
        {
            return ExperimentCommon.Run(
                task,
                z => CreateBot(z.ClassName, z.Params));
        }

 
    }


}