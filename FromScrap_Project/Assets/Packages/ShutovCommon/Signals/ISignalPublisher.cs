using System;
using UniRx;

namespace ShootCommon.Signals
{
    public interface ISignalPublisher
    {
        void Publish<TSignal>(TSignal signal)
            where TSignal: ISignal;
    }
}