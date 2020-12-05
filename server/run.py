import socket
import time
from urllib import parse
import json

err404 = '''
HTTP/1.x 404 Not Found
Content-Type: text/html\n
<html>
    <head>
        <title>404 Not Found</title>
    </head>
    <body>
        <center><h1><p>404<p>Not Found</h1></center>
    </body>
</html>'''.encode('UTF-8')

err500 = '''
HTTP/1.x 500 Server Error
Content-Type: text/html\n
<html>
    <head>
        <title>500 Server Error</title>
    </head>
    <body>
        <center><h1><p>500<p>Server Error</h1></center>
    </body>
</html>'''.encode('UTF-8')

head = '''
HTTP/1.x 200 OK
Content-Type: '''.encode('UTF-8')


def out(message='', msgtype=''):
    t = time.time()
    t = time.strftime("%Y-%m-%d %H:%M:%S.", time.localtime(t)) + str(int(t * 1000 % 1000))
    t = f"[{t}:{msgtype}] {message}"
    print(t)


def info(message=''):
    out(message, 'INFO ')


def warn(message=''):
    out(message, 'WARN ')


def error(message='', is_exit=False):
    out(message, 'ERROR')
    if is_exit:
        exit(1)


def get_res(file='', e=0):
    try:
        if e == 404:
            return err404, ''
        elif e == 500:
            return err500, ''
        with open(file, 'rb') as f:
            text = f.read()
        t = file.split('.')[-1]
        ret = head
        if t == "htm" or t == "html":
            ret += 'text/html'.encode('UTF-8')
        elif t == "jpg":
            ret += 'image/jpeg'.encode('UTF-8')
        elif t == "exe":
            ret += 'application/x-msdownload'.encode('UTF-8')
        else:
            ret += 'application/octet-stream'.encode('UTF-8')
        ret += "\n\n".encode('UTF-8') + text
        f.close()
    except FileNotFoundError:
        error("Cannot find resource: %s" % file)
        return err404, ''
    except Exception as e:
        error(f"Unknown error: {e}")
        return err500, ''
    return ret, head


def main(host, port, index):
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    try:
        s.bind((host, port))
    except OSError:
        error('Error: Port is used!', True)

    if host == '':
        info(f'Serving HTTP on port {port} ...')
    else:
        info(f'Serving HTTP on {host} port {port} ...')

    while True:
        s.listen(5)
        conn, addr = s.accept()
        request = str(conn.recv(1024)).strip()[2:-1]
        request = parse.unquote(request)
        info("==========")
        info("Connected from IP: %s Port: %s" % (addr[0], addr[1]))
        if request.strip() == '':
            info('Request: (None)')
            conn.sendall(get_res(e=404)[0])
        else:
            info('Request: %s' % request.split('\\r')[0])
            if len(request.split(' ')) < 2:
                conn.sendall(get_res(e=404)[0])
                conn.close()
                continue
            method, res = request.split(' ')[:2]

            if method == 'GET':
                if res == '/':
                    con_ret, head_ret = get_res(index)
                else:
                    con_ret, head_ret = get_res(res[1:])
                if head_ret == '':
                    info('Return: %s' % con_ret.decode('UTF-8').split('\n')[1])
                else:
                    info('Return: %s' % head_ret.decode('UTF-8').split('\n')[1])
                conn.sendall(con_ret)
            else:
                warn('Cannot use method "%s".' % method)
        conn.close()


def run():
    host = ''
    port = 80
    index = 'index.html'

    try:
        info("Loading settings.")
        f = open("settings.json", 'r')
        s = json.loads(f.read())
        f.close()
        if 'index' in s and s['index'] != "":
            index = s['index']
    except FileNotFoundError:
        warn("Cannot find setting file.")
        f = open("settings.json", 'w')
        f.write("{\n"
                "    \"index\":\"\"\n"
                "}")
        f.close()
    except json.JSONDecodeError or TypeError:
        warn("Cannot read setting file.")

    info("Index File: %s" % index)
    info("Loaded settings.")

    try:
        main(host, port, index)
    except KeyboardInterrupt:
        info('Quit.')


if __name__ == '__main__':
    run()
