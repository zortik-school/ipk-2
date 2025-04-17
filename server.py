#
# IPK25chat client TCP stream test
# author: vita v22.0
# date: 13. 4. 2025
# python 3.13.0
#
import socket
import os
import time

ADDRESS = "127.0.0.1"
PORT = 4567

def test(s: str) -> None:
    escaped = s.replace("\n", "\\n").replace("\r", "\\r")
    # print(f"Press enter to send \"{escaped}\": ", flush=True, end="")
    # input()
    csock.send(s.encode("ascii"))

sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.bind((ADDRESS, PORT))
sock.listen()
print("socket is bound and listening")
csock, _ = sock.accept()
print("connected")

while (True):
    print("1 - send reply ok")
    print("2 - send reply not ok")
    print("3 - send message with ending")
    print("4 - send part of message seporately")
    print("5 - send err")
    print("6 - send bye")
    print("7 - send invalid message")
    print("9 - exit program")
    pressed = input("Input a number: ")
    if (pressed == "1"):
        test("reply ok is Success.\r\n")
    elif (pressed == "2"):
        test("reply nok is Fail.\r\n")
    elif (pressed == "3"):
        test("msg from user1 is hello\r\n")
    elif (pressed == "4"):
        test("msg from user1 is 12")
        time.sleep(0.2)
        test("345\r\n")
    elif (pressed == "5"):
        test("err from server is Something went wrong.\r\n")
    elif (pressed == "6"):
        test("bye from user1\r\n")
    elif (pressed == "7"):
        test("wrong message sadge\r\n")
    elif (pressed == "9"):
        break
    os.system('clear')


csock.shutdown(socket.SHUT_RDWR)
csock.close()
sock.shutdown(socket.SHUT_RDWR)
sock.close()
print()