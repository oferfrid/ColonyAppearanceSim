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
		public TubeParameters TP;
		public double dt;
		
		int _LastT ;
		public double[] LastN; 
		
		public Tube(TubeParameters TP,double MaxTime)
		{
		this.TP = TP;
		dt=1;

			GrowDivision = new double[Convert.ToInt32(Math.Ceiling(MaxTime/dt)),TP.NumberOfStrains];
			
			
			//init to 0
			for (int i=0; i<GrowDivision.GetLength(0) ; i++)
			{
				for (int s=0; s<GrowDivision.GetLength(1) ; s++)
					GrowDivision[i,s]=0;
			}
			_LastT = 0;
			LastN = new double[TP.NumberOfStrains];
			for(int s=0;s<TP.NumberOfStrains;s++)
			{
				LastN[s] = TP.Strains[s].No;
			}
			
		}
		
		public  double[] Time
		{
			get {
				double[] Time = new double[GrowDivision.GetLength(0)];
				for(int i=0;i<LastT;i++)
				{
					Time[i] = dt*i;
				}
				return Time;
			}
		}
		public  double[,] NBacteria
		{
			get {
				//add row for time [time,strain 1 strain 2...]
				double[,] NBacteria = new double[LastT,TP.NumberOfStrains];
				
				
				//init the first cell to N0.
				for (int s=0;s<TP.NumberOfStrains;s++)
				{
					NBacteria[0,s] = TP.Strains[s].No + GrowDivision[0,s];
				}
				
				for(int i=1;i<LastT;i++)
				{
					NBacteria[i,0] = dt*i;
					for (int s=0;s<TP.NumberOfStrains;s++)
					{
						NBacteria[i,s] = NBacteria[i-1,s]+GrowDivision[i,s];
					}
				}
				
				
				return NBacteria;
			}
			
		}
		
			
		public  int LastT
		{
			get {
				return this._LastT;
			}
			set {
				int NewLastT = value;
				//calc the LastN from Old lastT to the new LastT
				double[] N = this.LastN;
				
				for(int t=this.LastT;t<=NewLastT;t++)
				{
					for (int s=0;s<TP.NumberOfStrains;s++)
					{
						N[s]+=GrowDivision[t,s];
					}
				}
				
				this._LastT = NewLastT+1;
				
			}
		}
		
				
	}
}

