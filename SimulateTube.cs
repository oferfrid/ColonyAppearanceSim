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
	/// Description of SimulateTube.
	/// </summary>
	public static class SimulateTube
	{
		
	
		private static  Tube CommuteLag(Tube T,double GrowKill)
			
		{
			//Get LagTime for each bacteria strain
			for(int s=0;s<T.TP.NumberOfStrains;s++)
			{
				//Calc Number of normal cels
				double N0Persisters = Math.Round((double)T.LastN[s]*T.TP.Strains[s].PersistersLevel);
				double N0Normal = T.LastN[s] - N0Persisters;
				
				double mulagNormal = 1.0/T.TP.Strains[s].LagMeanNormal;
				double mulagPersisters = 1.0/T.TP.Strains[s].LagMeanPersisters;
				
				//put normal first divition
				for(int i=0;i<N0Normal;i++)
				{
					double LagTime = Utils.RandDecayExponantial(mulagNormal) ;
					int DivInd = GetIndFromdouble(LagTime,T.dt);
					T.GrowDivision[DivInd+T.LastT,s]+=GrowKill;
				}
				//put Persisters first divition
				for(int i=0;i<N0Persisters;i++)
				{
					double LagTime = Utils.RandDecayExponantial(mulagPersisters);
					int DivInd = GetIndFromdouble(LagTime,T.dt);
					T.GrowDivision[DivInd+T.LastT,s]+=GrowKill;
				}
			}
			return T;
			
		}
		
		public static  Tube CommuteLagForGrow(Tube T)
		{
			 return CommuteLag( T, 1);
		}
		public static  Tube CommuteLagForKill(Tube T)
		{
			 return CommuteLag( T,-1);
		}
		
		public static Tube Kill(Tube T,double KillTime)
		{
		//usin kill whan bacteria divides
		CommuteLagForKill(T);
		int KillTimeInIndexs = (int)Math.Round(KillTime/T.dt)  ;
		
		//zero future divitions
			for(int tt = T.LastT + KillTimeInIndexs; tt<T.GrowDivision.GetLength(0);tt++)
			{
				for (int s=0;s<T.TP.NumberOfStrains;s++)
				{
					T.GrowDivision[tt,s]=0;
				}
			}
			
			T.LastT += KillTimeInIndexs;
		
		return T;
		}
		
		public static Tube GrowToNmax(Tube T)
		{
			double NTot=0;
			CommuteLagForGrow(T);
			for (int s=0;s<T.TP.NumberOfStrains;s++)
			{
				NTot+=T.LastN[s];
			}
			int t=T.LastT;
			
			do
			{
				for (int s=0;s<T.TP.NumberOfStrains;s++)
				{
					for(int i=0;i<T.GrowDivision[t,s];i++)
					{
						
						//add the next 2 divisions
						int ind;
						ind = GetDivisionTimeIndex(T.TP.Strains[s].DivLognormalParameters,T.dt);
						T.GrowDivision[t+ind,s]++;
						ind = GetDivisionTimeIndex(T.TP.Strains[s].DivLognormalParameters,T.dt);
						T.GrowDivision[t+ind,s]++;
						NTot++;
					}
				}
				t++;
				
			}while (NTot<T.TP.Nmax);
			
			T.LastT = t-1;
			
			//zero future divitions
			for(int tt = t; tt<T.GrowDivision.GetLength(0);tt++)
			{
				for (int s=0;s<T.TP.NumberOfStrains;s++)
				{
					T.GrowDivision[tt,s]=0;
				}
			}
			
			return T;
		}
		
		
		
		 private static int GetDivisionTimeIndex(Utils.LognormalParameters LP,double dt)
		{
			
			double DivTime = Utils.RandLogNormal(LP);
			
			int DivInd = GetIndFromdouble(DivTime,dt);
			return DivInd;
		}
		
		 private static int GetIndFromdouble(double Time,double dt)
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

