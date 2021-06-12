import re
import json
import requests
from pyquery import PyQuery as pq
from lxml import etree
from bs4 import BeautifulSoup

def get_html(url, chii_sid):
    headers = {"User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3100.0 Safari/537.36"}
    cookies = dict(chii_sid=chii_sid)
    result = requests.get(str(url), headers=headers, cookies=cookies)
    result.encoding = "utf-8"
    return result.text

def get_title(doc): # 获取标题
    title = str(doc('.nameSingle a').text())
    return title

def get_epid(doc, ep_num):
    ep_id = []
    for item in doc('.prg_list li a').items():
        ep_id.append((item.attr('href')))
    # total_num = len(doc('.prg_list li a').items())
    return ep_id[ep_num-1] # /ep/917953

def get_studio(doc):
    studio = ''
    for item in doc('#infobox li').items():
        if str(item('span').text()) == '製作:':
            studio = str(item('a').text())
    return studio

def get_sub_summary(doc):
    summary = ''
    summary = str(doc('#subject_summary').text())
    return summary

def get_tags(doc):
    tags = []
    for item in doc('.tagList.clearit').items():
        if str(item('span').text()) == '常用标签：':
            for i in item('a').items():
                tags.append(str(i.text()))
    return tags

def get_rate(doc):
    rate = '5.0'
    rate = str(doc('.global_score .number').text())
    return rate

def get_director(doc):
    director = ''
    for item in doc('#infobox li').items():
        if str(item('span').text()) == '导演:':
            director = str(item('a').text())
    return director

def get_start_date(doc):
    date = ''
    for item in doc('#infobox li').items():
        if str(item('span').text()) == '开始:':
            date = str(item('li').text())[4:]
    return date

def get_end_date(doc):
    date = ''
    for item in doc('#infobox li').items():
        if str(item('span').text()) == '结束:':
            date = str(item('li').text())[4:]
    return date

def get_ep_title(doc):
    title = ''
    doc('.mainWrapper .title').remove('a')
    title = str(doc('.mainWrapper .title').text())
    title2 = re.sub(r'ep.\d+', '', title)
    if len(title2) > 0: 
        return title2.strip(' ')
    else: return title

def get_air_date(doc):
    a = str(doc('.mainWrapper .tip').text())
    start = re.search(r'\d{4}-\d{2}-\d{2}', a)
    if start:
        s = start.group()
    else: s = ''
    return s

def get_runtime(doc): # 时长的格式不统一，暂时放弃这个元素
    a = str(doc('.mainWrapper .tip').text())
    time = re.search(r'\d{2}:\d{2}:\d{2}', a)
    if time:
        t = time.group()
    else: t = ''
    if t == '': runtime = 25
    else: runtime = int(t[0:2]) * 60 + int(t[3:5])
    return runtime

def get_ep_summary(doc):
    summary = ''
    a = doc('.mainWrapper .epDesc')
    a.remove('span')
    summary = str(a.text())
    return summary

def get_pic(doc):
    pic = ""
    a = doc('#bangumiInfo [align=center] a')
    pic = 'https:' + str(a.attr.href)
    return pic

def main(sub_id, ep_num, cookie):
    # config
    chii_sid = cookie #用来访问特殊页面的cookie

    sub_htmlcode = get_html('https://bgm.tv/subject/' + str(sub_id), chii_sid)        
    sub_doc = pq(sub_htmlcode)
    #sub_soup = BeautifulSoup(sub_htmlcode, 'lxml') 
    sub_title = get_title(sub_doc)
    if sub_title == '': # 判断cookie是否有效
        print('[-]未获取到页面，请检查cookie是否有效')
        return
    #print(sub_title)
    ep_id = get_epid(sub_doc, ep_num)
    ep_htmlcode = get_html('https://bgm.tv' + ep_id, chii_sid)
    ep_doc = pq(ep_htmlcode)
    sub_dic = {
        'title': sub_title,
        'studio': get_studio(sub_doc),
        'summary': get_sub_summary(sub_doc),
        'tags': get_tags(sub_doc),
        'rate': get_rate(sub_doc),
        'director': get_director(sub_doc),
        'start_date': get_start_date(sub_doc),
        'end_date': get_end_date(sub_doc),
        'year': get_start_date(sub_doc)[0:4],
        'pic_url': get_pic(sub_doc) #考虑到bgm的图片质量较差，暂不考虑
    }
    ep_dic = {
        'title': get_ep_title(ep_doc),
        'air_date': get_air_date(ep_doc), 
        'runtime': get_runtime(ep_doc),        # 如识别不到就默认25mins
        'summary':get_ep_summary(ep_doc),
    }
    '''
    for key in sub_dic:
        print(key, ":", sub_dic[key])
    for key in ep_dic:
        print(key, ":", ep_dic[key])
    '''
    sub_data = json.dumps(sub_dic, ensure_ascii=False, sort_keys=True, indent=4, separators=(',', ':'), )  # .encode('UTF-8')
    ep_data = json.dumps(ep_dic, ensure_ascii=False, sort_keys=True, indent=4, separators=(',', ':'), )  # .encode('UTF-8')
    return sub_data, ep_data

if __name__ == '__main__':
    cookie = ""
    headers = {"User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3100.0 Safari/537.36"}
    cookies = {}
    for line in cookie.split(";"):
        print(line)
        if line.find("=") != -1:
            name,value = line.strip().split("=")
            cookies[name] = value
    url = "https://bgm.tv/subject/311528"
    result = requests.get(str(url), headers=headers, cookies=cookies)
    result.encoding = "utf-8"
    sub_doc = pq(result.text)
    #sub_soup = BeautifulSoup(sub_htmlcode, 'lxml') 
    sub_title = get_title(sub_doc)
    if sub_title == '': # 判断cookie是否有效
        print('[-]未获取到页面，请检查cookie是否有效')
    else:
        print(sub_title)
