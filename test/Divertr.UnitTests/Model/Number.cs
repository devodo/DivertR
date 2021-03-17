﻿using System;

namespace DivertR.UnitTests.Model
{
    public class Number : INumber
    {
        private readonly Func<int, int> _numberFactory;

        public Number() : this(i => i)
        {
        }

        public Number(Func<int, int> numberFactory)
        {
            _numberFactory = numberFactory;
        }

        public int GetNumber(int input)
        {
            return _numberFactory.Invoke(input);
        }

        public void RefNumber(ref int input)
        {
            input = _numberFactory(input);
        }
        
        public void RefArrayNumber(ref int[] inputs)
        {
            var replacement = new int[inputs.Length];
            
            for (var i = 0; i < inputs.Length; i++)
            {
                replacement[i] = _numberFactory.Invoke(inputs[i]);
            }

            inputs = replacement;
        }

        public string GenericNumber<T>(T arg, int input)
        {
            return $"{arg} - {_numberFactory.Invoke(input)}";
        }

        public void ArrayNumber(int[] inputs)
        {
            for (var i = 0; i < inputs.Length; i++)
            {
                inputs[i] = _numberFactory.Invoke(inputs[i]);
            }
        }
    }
}