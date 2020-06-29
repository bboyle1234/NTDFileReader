# NTDFileReader

**NinjaTrader 8**

 To read ticks from a NinjaTrader 8 .ncd file
```csharp
// 1. Get a stream for reading the tick file
using var tickFileStream = File.Open("pathToTickDataFile.ncd");
// 2. Enumerate through each tick
foreach(NCDTick tick in NCDUtility.ReadTicks(tickFileStream)) { 
  // do something
}
```


 To read minute bars from a NinjaTrader 8 .ncd file
```csharp
// 1. Get a stream for reading the tick file
using var minuteFileStream = File.Open("pathToMInuteDataFile.ncd");
// 2. Enumerate through each tick
foreach(NCDMinute tick in NCDUtility.ReadTicks(minuteFileStream)) { 
  // do something
}
```

**NinjaTrader 7**

```csharp
// To read ticks from the NinjaTrader 7 .ntd file
// 1. Get a stream for reading the tick file
using var stream = File.Open("pathToTickDataFile.ntd");
// 2. Enumerate through each tick
foreach(NTDTick tick in NTDUtility.ReadTicks(stream)) { 
  // do something
}
```

# Nuget
`Install-Package NTDFileReader`

# Thanks
https://www.bigmiketrading.com/ninjatrader-programming/7396-ntd-file-specification.html

https://www.bigmiketrading.com/elite-circle/6802-gomrecorder-2-a-37.html#post112912

 https://github.com/jrstokka/NinjaTraderNCDFiles/blob/master/NCDFile.cs