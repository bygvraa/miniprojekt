using System;

namespace Models
{
	public class Subject
	{
		// Properties
		public int Id { get; set;}
		public string Name { get; set;}

		// Konstruktør
		public Subject(string name)	{
			Name = name;
		}
	}
}
