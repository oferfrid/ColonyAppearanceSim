/*
 * Created by SharpDevelop.
 * User: owner
 * Date: 22/07/2011
 * Time: 13:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace IritSimulation
{
	/// <summary>
	/// Description of TubeParameters.
	/// </summary>
	public struct TubeParameters //: IEquatable<TubeParameters>
	{
		public double Nmax;
		public int NumberOfStrains;
		public StrainParameters[] Strains;
		
		public TubeParameters(
			double Nmax,
			StrainParameters[] Strains
		)
			
		{
			this.Nmax = Nmax;
			this.Strains = Strains;
			NumberOfStrains = Strains.Length;
		}
	}
}
