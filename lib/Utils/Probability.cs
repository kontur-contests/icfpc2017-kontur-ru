using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ProbabilityMethods
{
	[DataContract]
	public struct Probability
	{
		[DataMember]
		private double _logProbability;

		public static readonly Probability Zero = Probability.GetLogProbability(0);
		public static readonly Probability One = Probability.GetLogProbability(1);
		private Probability(double logProbability)
		{
			_logProbability = logProbability;
		}

		private static Probability GetLogProbability(double probability)
		{
			return new Probability(Math.Log(probability));
		}

		public static Probability From(double probability)
		{
			return GetLogProbability(probability);
		}

		public static Probability FromLogProbability(double logProbability)
		{
			return new Probability(logProbability);
		}

		public double GetProbability()
		{
			return Math.Exp(_logProbability);
		}

		public double GetLogProbabilityValue()
		{
			return _logProbability;
		}

		public static Probability Max(Probability prob1, Probability prob2)
		{
			return prob1 > prob2 ? prob1 : prob2;
		}


		public static bool operator >(Probability prob1, Probability prob2)
		{
			return prob1._logProbability > prob2._logProbability;
		}

		public static bool operator <(Probability prob1, Probability prob2)
		{
			return prob1._logProbability < prob2._logProbability;
		}

		public static bool operator ==(Probability prob1, Probability prob2)
		{
			if (double.IsNegativeInfinity(prob1._logProbability) && double.IsNegativeInfinity(prob2._logProbability))
				return true;
			return Math.Abs(prob1._logProbability - prob2._logProbability) < 0.0001;
		}

		public static bool operator !=(Probability prob1, Probability prob2)
		{
			return !(prob1 == prob2);
		}

		public static Probability operator ~(Probability prob)
		{
			return new Probability(Math.Log(1 - prob.GetProbability()));
		}

		public static Probability operator *(Probability prob1, Probability prob2)
		{
			return new Probability(prob1._logProbability + prob2._logProbability);
		}

		public static Probability operator *(Probability prob1, double prob2)
		{
			return prob1 * GetLogProbability(prob2);
		}
		public static Probability operator *(double prob1, Probability prob2)
		{
			return prob2 * prob1;
		}

		public static Probability operator /(Probability prob1, Probability prob2)
		{
			return new Probability(prob1._logProbability - prob2._logProbability);
		}

		public static Probability operator /(Probability prob1, double prob2)
		{
			return prob1 / GetLogProbability(prob2);
		}

		public static Probability operator +(Probability prob1, Probability prob2)
		{
			if (prob1._logProbability < prob2._logProbability)
			{
				var temp = prob2;
				prob2 = prob1;
				prob1 = temp;
			}
			if (double.IsNegativeInfinity(prob1._logProbability))
				return prob1;
			return new Probability(prob1._logProbability + Math.Log(1 + Math.Exp(prob2._logProbability - prob1._logProbability)));
		}

		public static Probability Sum(IList<Probability> probs)
		{
			if (probs.Count == 0)
				return Zero;

			var maxLogProb = probs.Select(prob => prob._logProbability).Max();

			double sum = 0;

			for (int i = 0; i < probs.Count; i++)
			{
				sum += Math.Exp(probs[i]._logProbability - maxLogProb);
			}
			return new Probability(maxLogProb + Math.Log(sum));
		}

		public Probability Pow(double num)
		{
			return new Probability(num < 1e-50 ? 0 : num * _logProbability);
		}

		public Probability NoisyOr(Probability probability)
		{
			return ~(~this * ~probability);
		}

		public override string ToString()
		{
			return GetProbability().ToString();
		}
	}

	public static class LogIEnumerableExtension
	{
		public static Probability Sum(this IEnumerable<Probability> probs)
		{
			return probs.Aggregate(Probability.Zero, (aggr, prob) => aggr + prob);
		}

		public static Probability Sum(this IList<Probability> probs)
		{
			return Probability.Sum(probs);
		}

		public static Probability Prod(this IEnumerable<Probability> probs)
		{
			return probs.Aggregate(Probability.One, (aggr, prob) => aggr * prob);
		}

		public static Probability Max(this IEnumerable<Probability> probs)
		{
			return probs.Aggregate(Probability.Zero, (aggr, prob) => aggr < prob ? prob : aggr);
		}
	}
}
