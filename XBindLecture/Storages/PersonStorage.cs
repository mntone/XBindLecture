using System.Linq;
using XBindLecture.DataSets;

namespace XBindLecture.Storages
{
	public static class PersonStorage
	{
		public static Person[] _persons = {
			new Person("Taro", 14, SexType.Male),
			new Person("Jiro", 8, SexType.Male),
			new Person("Kazuo", 18, SexType.Male),
			new Person("Hanako", 10, SexType.Female),
			new Person("Kazuko", 16, SexType.Male),
			new Person("Inkling", 14, SexType.Unknown),
		};

		public static Person[] GetPersons() => _persons;
		public static Person GetPerson(string name)
		{
			return _persons.Where(p => p.Name == name).Single();
		}
	}
}