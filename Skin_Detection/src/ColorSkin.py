from PIL import  Image
import cv2
image = Image.open('outputprojet.jpg', 'r')
pixlist = list(image.getdata())
listR = [len(pixlist)]
listG = [len(pixlist)]
listB = [len(pixlist)]
cptR = 0l
cptG = 0
cptB = 0

for i in range (len(pixlist)):
    if pixlist[i][0] in range(6,255):
        listR.append(pixlist[i][0])
    if pixlist[i][1] in range(6,255):
        listG.append(pixlist[i][1])
    if pixlist[i][2] in range(6,255):
        listB.append(pixlist[i][2])


for r in range (len(listR)) :

    cptR += listR[r]

for g in range (len(listG)):
    cptG += listG[g]

for b in range (len(listB)):
    cptB += listB[b]


moyR = cptR/len(listR)
moyG = cptG/len(listG)
moyB = cptB/len(listB)

averageColor = (int(moyR), int(moyG), int(moyB))
imout = Image.new("RGB", (300,300),averageColor )
imout.save('avgcolor.jpg')










