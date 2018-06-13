using System;
using UnityEngine;
using System.Collections.Generic;

namespace Kernel.Core
{
	public interface IConfigIdentity
	{
		string Id { get; }
	}
	
	public abstract class Config : ScriptableObject
	{
		public const int Order = 1000;
	}
	
	public abstract class Config<T> : Config, IEnumerable<T> where T : new()
	{
		[SerializeField]
		private List<T> _data;

		public List<T> Data { get { return _data; } }

		public IEnumerator<T> GetEnumerator()
		{
			return Data.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Data.GetEnumerator();
		}
	}
	
	public abstract class ConfigIdentity<T> : Config<T> where T : IConfigIdentity, new()
	{
		private Dictionary<string, T> _identityData = null;

		public Dictionary<string, T> IdentityData
		{
			get
			{
				if (_identityData == null)
				{
					_identityData = new Dictionary<string, T>();
					foreach (var data in Data)
					{
						if (!_identityData.ContainsKey(data.Id))
						{
							_identityData.Add(data.Id, data);
						}
						else
						{
							Debug.LogErrorFormat("Key \"{0}\" already exists", data.Id);
						}
					}
				}
				return _identityData;
			}
		}

		public T GetById(string id)
		{
			T data;
			if (IdentityData.TryGetValue(id, out data)) return data;
			return default(T);
		}
	}
	
	public abstract class ConfigMultiIdentity<T> : Config<T> where T : IConfigIdentity, new()
	{
		[SerializeField]
		protected Dictionary<string, List<T>> _dictionary = null;

		public Dictionary<string, List<T>> Dictionary
		{
			get
			{
				if (_dictionary == null)
				{
					_dictionary = new Dictionary<string, List<T>>();
					foreach (var data in Data)
					{
						if (!_dictionary.ContainsKey(data.Id)) _dictionary[data.Id] = new List<T>();
						_dictionary[data.Id].Add(data);
					}
				}
				return _dictionary;
			}
		}

		public List<T> GetById(string id)
		{
			List<T> data;
			if (Dictionary.TryGetValue(id, out data)) return data;
			return null;
		}
	}
}
