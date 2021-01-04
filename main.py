import Anime_Data_Capture
import Voice_Data_Capture
text = "请选择模式：\n【1】动画模式\n【2】音声模式\n"
choice = input(text)
if choice == '1':
    print("选择了动画模式")
    Anime_Data_Capture.main()
elif choice == '2':
    print("选择了音声模式")
    Voice_Data_Capture.main()
else:
    input('请输入正确的数字，按enter退出')