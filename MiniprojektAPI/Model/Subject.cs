using System;

namespace Model
{
	public class Subject
	{
		// Properties
		public int Id { get; set;}
		public string Name { get; set;}


		// Konstruktør
		public Subject(string name)
		{
			this.Name = name;
		}
	}
}
