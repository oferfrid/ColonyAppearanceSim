/*
 * Created by SharpDevelop.
 * User: oferfrid
 * Date: 23/07/2011
 * Time: 14:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace IritSimulation
{
	/// <summary>
	/// Description of StrainParameters.
	/// </summary>
	public struct StrainParameters
	{
		public string Name;
		
		public double No;
		public double PersistersLevel;
		
		//lag parameters
		public double LagMeanNormal;
		public double LagMeanPersisters;
		
		//division parameters
		public double DivMean;
		public double DivVar;
		public StrainMutationParameters[] StrainMutationParameters;
		
		Utils Utils;
		
		
		public Utils.LognormalParameters DivLognormalParameters ;
		
		public StrainParameters(
			string Name,
			double No,
			double PersistersLevel,
			double LagMeanNormal,
			double LagMeanPersisters,
			double DivMean,
			double DivVar,
			StrainMutationParameters[] StrainMutationParameters
		)
		{
			Utils = new Utils();
			
			this.Name = Name;
			this.No = No  ;
			this.PersistersLevel =  PersistersLevel ;
			this.LagMeanNormal =  LagMeanNormal ;
			this.LagMeanPersisters =  LagMeanPersisters ;
			this.DivMean = DivMean  ;
			this.DivVar = DivVar  ;
			DivLognormalParameters = Utils.CommuteLognormalParameters(DivMean,DivVar);
			this.StrainMutationParameters = StrainMutationParameters ;
		}
		
		public StrainParameters(
			string Name,
			double No,
			double PersistersLevel,
			double LagMeanNormal,
			double LagMeanPersisters,
			double DivMean,
			double DivVar
		)
		{
			Utils = new Utils();
			
			this.Name = Name;
			this.No = No  ;
			this.PersistersLevel =  PersistersLevel ;
			this.LagMeanNormal =  LagMeanNormal ;
			this.LagMeanPersisters =  LagMeanPersisters ;
			this.DivMean = DivMean  ;
			this.DivVar = DivVar  ;
			DivLognormalParameters = Utils.CommuteLognormalParameters(DivMean,DivVar);
			StrainMutationParameters= new StrainMutationParameters[0];
		}


	}
	
	public struct StrainMutationParameters
	{
		public int Tostrain;
		public double MutationRatePerDivition;
		public double MutationRatePerTimeUnit;
		
		public StrainMutationParameters(
			int Tostrain,
			double MutationRatePerDivition,
			double MutationRatePerTimeUnit
		)
		{
			this.Tostrain = Tostrain;
			this.MutationRatePerDivition = MutationRatePerDivition;
			this.MutationRatePerTimeUnit=MutationRatePerTimeUnit;
		}
		
	}
	
	
}
