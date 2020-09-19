import json
import requests

headers={"User-Agent" : "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.102 Safari/537.36",
  "Accept" : "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8",
  "Accept-Language" : "en-us",
  "Connection" : "keep-alive",
  "Accept-Charset" : "GB2312,utf-8;q=0.7,*;q=0.7"}

server = "https://api.bgm.tv"
search = "/search/subject/"

def get_id(keyword):
    r = requests.get(server + search + keyword + '?type=2', headers = headers)
    temp = r.json()
    subject_id = temp['list'][0]['id'] # 默认搜索结果第一位为正确结果
    return subject_id


if __name__ == '__main__':
    print(get_id('ID:INVADED'))
    print(get_id('進撃の巨人'))   # 分季的存在一些问题