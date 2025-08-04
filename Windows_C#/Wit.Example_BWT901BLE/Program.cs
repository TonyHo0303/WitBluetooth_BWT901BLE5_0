using System;
using System.Threading;
using Wit.Bluetooth.WinBlue.Interface;
using Wit.Bluetooth.WinBlue.Utils;
using Wit.SDK.Device.Device.Device.DKey;
using Wit.SDK.Modular.WitSensorApi.Modular.BWT901BLE;

namespace Wit.Example_BWT901BLE
{
    static class Program
    {
        static bool deviceConnected = false;
        static DateTime startTime;

        static void Main()
        {
            IWinBlueManager bluetoothManager = WinBlueFactory.GetInstance();
            bluetoothManager.OnDeviceFound += OnDeviceFound;

            startTime = DateTime.Now;
            bluetoothManager.StartScan();

            // 每秒输出“搜索中，时间……”
            while (!deviceConnected)
            {
                Console.WriteLine($"搜索中，时间{DateTime.Now:HH:mm:ss}");
                Thread.Sleep(1000);
            }

            // 等待设备连接和处理完成
            Thread.Sleep(2000);
            bluetoothManager.StopScan();
            Console.WriteLine("程序结束");
        }

        static void OnDeviceFound(string mac, string name)
        {
            // 只连接WT开头的设备
            if (deviceConnected) return;
            if (string.IsNullOrEmpty(name) || !name.StartsWith("WT")) return;

            deviceConnected = true;

            //Console.WriteLine($"发现设备: {name} [{mac}]");
            var device = new Bwt901ble(mac, name);
            device.Open();
            Console.WriteLine("device open");


            // 等待设备连接
            Thread.Sleep(1000);

            Console.WriteLine("连接成功");

            // 获取设备ID（假设GetDeviceName为设备ID）
            string deviceId = device.GetDeviceName();
            Console.WriteLine($"设备ID: {deviceId}");

            // 数据读取线程
            Thread dataThread = new Thread(() =>
            {
                while (device.IsOpen())
                {

                    string tempkey = "AccX"; string tempunit = "m/s²"; string tempname = "X轴加速度";
                    double? accX = device.GetDeviceData(new DoubleKey(tempkey, tempname, tempunit));
                    Console.WriteLine("HAHAHA");
                    if (accX!=null)
                    {
                        Console.WriteLine("WUWUWU");
                        Console.WriteLine($"X轴加速度: {accX.Value}");
                    }
                    else
                    {
                        Console.WriteLine("NULL");
                    }
                        Thread.Sleep(100);
                }
                device.Close();
                Console.WriteLine("结束连接");
            });
            dataThread.IsBackground = true;
            dataThread.Start();

        }
    }
}