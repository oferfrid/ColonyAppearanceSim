/*
 * Created by SharpDevelop.
 * User: oferfrid
 * Date: 02/08/2009
 * Time: 12:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace IritSimulation
{
	/// <summary>
	/// Description of Tube.
	/// </summary>
	public class Tube
	{
		
		
		public double[,] GrowDivision;
		

		TubeParameters TP;
		
		public double dt =1;
		public double t;
		
		public Tube(TubeParameters TP,double MaxTime)
		{
			this.TP = TP;

			GrowDivision = new double[Convert.ToInt32(Math.Ceiling(MaxTime/dt)),TP.NumberOfStrains];
			
			
			//init to 0
			for (int i=0; i<GrowDivision.GetLength(0) ; i++)
			{
				for (int s=0; s<GrowDivision.GetLength(1) ; s++)
					GrowDivision[i,s]=0;
			}
			
			
		}
		
		public double[,] CommuteLag()
			
		{
			//Get LagTime for each bacteria strain
			for(int s=0;s<TP.NumberOfStrains;s++)
			{
				//Calc Number of normal cels
				double N0Persisters = Math.Round((double)TP.Strains[s].No*TP.Strains[s].PersistersLevel);
				double N0Normal = TP.Strains[s].No - N0Persisters;
				
				double mulagNormal = 1.0/TP.Strains[s].LagMeanNormal;
				double mulagPersisters = 1.0/TP.Strains[s].LagMeanPersisters;
				
				//put normal first divition
				for(int i=0;i<N0Normal;i++)
				{
					double LagTime = Utils.RandDecayExponantial(mulagNormal) ;
					int DivInd = GetIndFromdouble(LagTime);
					GrowDivision[DivInd,s]++;
				}
				//put Persisters first divition
				for(int i=0;i<N0Persisters;i++)
				{
					double LagTime = Utils.RandDecayExponantial(mulagPersisters);
					int DivInd = GetIndFromdouble(LagTime);
					GrowDivision[DivInd,s]++;
				}
			}
			return GrowDivision;
			
		}
		
		
		public double[,] Kill(double KillTime)
		{
		//usin kill whan bacteria divides
		
		}
		
		public double[,] GrowToNmax()
		{
			double NTot=0;
			
			for (int s=0;s<TP.NumberOfStrains;s++)
			{
				NTot+=TP.Strains[s].No;
			}
			int t=0;
			
			do
			{
				for (int s=0;s<TP.NumberOfStrains;s++)
				{
					for(int i=0;i<GrowDivision[t,s];i++)
					{
						
						//add the next 2 divisions
						int ind;
						ind = GetDivisionTimeIndex(TP.Strains[s].DivLognormalParameters);
						GrowDivision[t+ind,s]++;
						ind = GetDivisionTimeIndex(TP.Strains[s].DivLognormalParameters);
						GrowDivision[t+ind,s]++;
						NTot++;
					}
				}
				t++;
				
			}while (NTot<TP.Nmax);
			
			//zero future divitions
			for(int tt = t; tt<GrowDivision.GetLength(0);tt++)
			{
				for (int s=0;s<TP.NumberOfStrains;s++)
				{
					GrowDivision[tt,s]=0;
				}
			}

			return GrowDivision;
		}
		
		public  double[,] NBacteria
		{
			get {
				//add row for time [time,strain 1 strain 2...]
				double[,] NBacteria = new double[GrowDivision.GetLength(0),TP.NumberOfStrains +1];
				
				NBacteria[0,0] = dt*0;
				//init the first cell to N0.
				for (int s=0;s<TP.NumberOfStrains;s++)
				{
					NBacteria[0,s+1] = TP.Strains[s].No + GrowDivision[0,s];
				}
				
				for(int i=1;i<GrowDivision.GetLength(0);i++)
				{
					NBacteria[i,0] = dt*i;
					for (int s=0;s<TP.NumberOfStrains;s++)
					{
						NBacteria[i,s+1] = NBacteria[i-1,s+1]+GrowDivision[i,s];
					}
				}
				
				
				return NBacteria;
			}
			
		}
		
		
		private int GetDivisionTimeIndex(Utils.LognormalParameters LP)
		{
			
			double DivTime = Utils.RandLogNormal(LP);
			
			int DivInd = GetIndFromdouble(DivTime);
			return DivInd;
		}
		
		private int GetIndFromdouble(double Time)
		{
			int ind;
			if (Time == double.PositiveInfinity)
			{
				throw new Exception("PositiveInfinity!");
			}
			else
			{
				ind = Convert.ToInt32(Math.Round((Time/dt)));
			}
			if (ind<0)
			{
				throw new Exception("Negative index!");
			}
			return ind;
		}
		
		
		
	}
}

