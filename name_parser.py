import os
import re

# 实现对文件名进行分析，提取出动画名，季数，集数


def name_parser(filepath):
    filepath = os.path.basename(filepath)

    try:        # 还没考虑SP的情况
        info = re.search(r'S[0-9]+E[0-9]+', filepath).group()
        s_num = int(re.search(r'S[0-9]+', info).group().strip('S'))
        ep_num = int(re.search(r'E[0-9]+', info).group().strip('E'))
        name = str(re.sub(r' S[0-9]+E[0-9]+.[a-zA-Z0-9_]*', '', filepath))
        # print(name, s_num, ep_num)
        return name, s_num, ep_num
    except Exception as e:
        print('[-]' + str(e))
        return

if __name__ == "__main__":
    name_parser('usr/temp/Hanzawa Naoki S02E01.mp4')
    name_parser('./ID:INVADED S01E99.mp4')
    name_parser('C:¥Users¥/ID:INVADED S01E01.mp4')
    name_parser('ID:INVADED S01E01.mp4')