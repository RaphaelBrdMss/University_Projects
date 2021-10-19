# import

import imutils
import numpy as np
import cv2
from PIL import  Image

# ap = argparse.ArgumentParser()
# ap.add_argument("-i", "--image", "path")
# args = vars(ap.parse_args())
# ([139,69,19], [255,255,255]),
lower_HSV = np.array([0, 100, 60], dtype="uint8")
upper_HSV = np.array([20, 255, 255], dtype="uint8")
lower_YCbCr = np.array((0, 138, 67), dtype="uint8")
upper_YCbCr = np.array((255, 173, 133), dtype="uint8")

img = cv2.imread('IMG_9119.jpg')


frame = imutils.resize(img, 400)
HSV = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)
YCbCr = cv2.cvtColor(frame, cv2.COLOR_BGR2YCR_CB)
binary_mask = HSV
HSV_mask = cv2.inRange(HSV, lower_HSV, upper_HSV)
YCbCr_mask = cv2.inRange(YCbCr, lower_YCbCr, upper_YCbCr)
binary_mask = cv2.add(HSV_mask, YCbCr_mask)

foregrand = cv2.erode(binary_mask, None, iterations=1)
dilat = cv2.dilate(binary_mask, None, iterations=1)
ret, background = cv2.threshold(dilat, 1, 128, cv2.THRESH_BINARY)
balise = cv2.add(foregrand, background)
balise32 = np.int32(balise)

cv2.watershed(frame, balise32)
u8 = cv2.convertScaleAbs(balise32)

ret, final_mask = cv2.threshold(u8, 0, 255, cv2.THRESH_BINARY+cv2.THRESH_OTSU)
ouput = cv2.bitwise_and(frame, frame, mask=final_mask)
cv2.imwrite("outputprojet.jpg", ouput)
cv2.waitKey(0)
cv2.destroyAllWindows("skinShow")
