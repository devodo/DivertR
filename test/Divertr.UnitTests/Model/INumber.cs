﻿using System.Threading.Tasks;

namespace DivertR.UnitTests.Model
{
    public interface INumber
    {
        int GetNumber(int input);
        string GenericNumber<T>(T arg, int input);
        void ArrayNumber(int[] inputs);
        void RefArrayNumber(ref int[] inputs);
        void RefNumber(ref int input);
        void OutNumber(out int output);
        
        Task<int> GetNumberAsync(int input);
        ValueTask<int> GetNumberValueAsync(int input);
    }
}