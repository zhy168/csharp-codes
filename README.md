# csharp-codes
C# winforms codes
A sensor connected with Linux computer send its data to  Windows winform by Socket.Winform display data by ScottPlot.
一个激光测距传感器接在Linux电脑上，串口解析数据后用Socket发送给winform上位机，用Scottplot绘图。其中，ScottPlot原生好像无法用滑动窗口方式显示数据，demo上虽然可以，但是类中没有List 类似removeAt的方法，重写类，继承原来的类，添加了removeAt方法,可以在用列表更新数据时删除列表前面的数据，实现数据滚动（scroll）
