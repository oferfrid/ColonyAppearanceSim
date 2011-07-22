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
	/// Description of Colony.
	/// </summary>
	public class Colony
	{
		
		public double LagTime;
		public double GrowTime;
		
		public double[] GrowDivision;
		
		
		double dt = 0.001;
		double tu;
		double N0;
		double Sigma;
		
		public Colony(double MaxTime,int N0,double tu,double Sigma)
		{

			this.tu = tu;
			this.Sigma = Sigma;
			 GrowDivision = new double[Convert.ToInt32(Math.Ceiling(MaxTime/dt))];
			 //GrowDivision = new double[Convert.ToInt32( Math.Ceiling(GrowTime/dt))];
			//GrowDivision = new double[Convert.ToInt32(GrowTime)];
			//init array
			
			this.N0 = N0;	
			for (int i=0; i<GrowDivision.Length ; i++)
			{
				GrowDivision[i]=0;	
			}
			for (int i=0;i<N0;i++)
			{
				int ind;
				ind = GetDivisionTimeIndex(tu);
				if (ind < GrowDivision.Length)
					GrowDivision[ind]++;
			}
			
		}
		public double LagZero()
		{
			LagTime = 0;
			return LagTime;
		}

		public double Lag(double muLag1,
		                  double sigLag1,
		                  double muLag2,
		                  double sigLag2,
		                  double ratio12)
			
		{
			//Get LagTime from decaying exponential distribution
			double RandRatio = Utils.RandUniform(1);
			if (ratio12>RandRatio)
			{
				//if 1
				LagTime = Utils.RandDecayExponantial(muLag1);
				
			}
			else
			{
				//if 2
				LagTime = Utils.RandDecayExponantial(muLag2);
			}
			return LagTime;
		}
		
		
		public double LagDoubleNormal(double muLag1,
		                              double sigLag1,
		                              double muLag2,
		                              double sigLag2,
		                              double ratio12)
		{
			//Get LagTime from double gaussian distribution
			double RandRatio = Utils.RandUniform(1);
			if (ratio12>RandRatio)
			{
				//if 1
				LagTime = Utils.RandNormal(sigLag1)+muLag1;
				
			}
			else
			{
				//if 2
				LagTime = Utils.RandNormal(sigLag2)+muLag2;
			}
			
			return LagTime;
		}

		public double LagDelta(double muLag1)
		{
			//Get LagTime from delta function
			LagTime = muLag1;
			
			return LagTime;
		}
		
		public double GrowtoSize(double Size)
			// returns the time in minutes it takes a colony 
			// with N0 initial cells to get to Size.
		{
			double N=N0;
			int t=0;
			do
			{
				for(int i=0;i<GrowDivision[t];i++)
				{
					int ind;
					ind = GetDivisionTimeIndex(tu);
					GrowDivision[t+ind]++;
					ind = GetDivisionTimeIndex(tu);
					GrowDivision[t+ind]++;
					N++;
				}
				t++;
				
			}while (N<Size);
			
			GrowTime = ((double)t)*dt;

			return GrowTime;
		}
		
		public double GrowtoTime(double time)
		{
			double N=N0;
			int t=0;
			do
			{
				for(int i=0;i<GrowDivision[t];i++)
				{
					int ind;
					for (int m=0;m<2;m++) {
						ind = GetDivisionTimeIndex(tu);
						if (t+ind < GrowDivision.Length)
							GrowDivision[t+ind]++;
								
					}
					N++;

						
					//ind = GetDivisionTimeIndex(tu);
					//GrowDivision[t+ind]++;
					//N++;
				}
				t++;
				
			}
			while (((double)t)*dt<=time);
			
			return N;
		}
		
		
		public double GetVisibleTime()
		{
			return LagTime+GrowTime;
		}

		public double GetVisibleTime(System.IO.StreamWriter SR)
		{
			for (int i=1; i<GrowDivision.Length ; i++)
			{
				SR.WriteLine(GrowDivision[i]);
			}
			return LagTime+GrowTime;
		}
		
		private int GetDivisionTimeIndex(double tau)
		{
			//double DivTime = Utils.RandBiNormal(20,2,100,2,0.2);
			//double DivTime = Utils.RandBiNormal(8,2,100,2,0.2);
			
			//double DivTime = Utils.RandNormal(tau,Sigma);
			double DivTime = Utils.RandNormal(tau,Sigma);
			//double DivTime = Utils.RandDecayExponantial(1.0/tau);
			//double DivTime = Utils.RandDecayExponantial(1.0/tau);
			//RandExponantial(double Nf,double N0
			//double DivTime = muDouble;
			//double DivTime = Utils.RandUniform(muDouble-sigDouble, muDouble+sigDouble);
			int DivInd = GetIndFromdouble(DivTime);
			return DivInd;
		}
		
		private int GetIndFromdouble(double Time)
		{
			int ind = 0 ;
			if (Time == double.PositiveInfinity)
			{
				ind = 0; //GrowDivision.Length;
			}
			else
			{
				ind = Convert.ToInt32(Math.Round((Time/dt)));
			}
			if (ind<0)
			{
				ind = 0;
			}
			return ind;
		}
		
		
		
	}
}

