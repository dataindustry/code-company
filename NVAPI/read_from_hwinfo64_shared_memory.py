import struct
from multiprocessing import shared_memory

import construct as cstruct
from construct import Struct, Int32un, Long

memory = shared_memory.SharedMemory('Global\\HWiNFO_SENS_SM2')

# size = 44
sensor_element_struct = Struct(
    'dwSignature' / Int32un,
    'dwVersion' / Int32un,
    'dwRevision' / Int32un,
    'poll_time' / Long,
    'dwOffsetOfSensorSection' / Int32un,
    'dwSizeOfSensorElement' / Int32un,
    'dwNumSensorElements' / Int32un,
    'dwOffsetOfReadingSection' / Int32un,
    'dwSizeOfReadingElement' / Int32un,
    'dwNumReadingElements' / Int32un,
)

sensor_element = sensor_element_struct.parse(memory.buf[0:Struct.sizeof(sensor_element_struct)])
print(sensor_element)

# unused. not work but can be used to find variable name
# reading_element_struct = Struct(
#     'tReading' / cstruct.Int32un,
#     'dwSensorIndex' / cstruct.Int32un,
#     'dwReadingID' / cstruct.Int32un,
#     'szLabelOrig' / cstruct.PaddedString(128, encoding='utf-8'),  # error
#     'szLabelUser' / cstruct.PaddedString(128, encoding='utf-8'),  # error
#     'szUnit' / cstruct.PaddedString(16, encoding='mbcs'),  # error
#     'Value' / cstruct.Double,
#     'ValueMin' / cstruct.Double,
#     'ValueMax' / cstruct.Double,
#     'ValueAvg' / cstruct.Double
# )

fmt = '=III128s128s16sdddd'

reading_element_struct = struct.Struct(fmt)
offset = sensor_element.dwOffsetOfReadingSection
length = sensor_element.dwSizeOfReadingElement

for index in range(sensor_element.dwNumReadingElements):
    reading = reading_element_struct.unpack(memory.buf[offset + index * length: offset + (index + 1) * length])
    print(reading[0], reading[1], reading[2])
    print(reading[3].replace(b'\00', b'').decode('utf-8'),
          reading[4].replace(b'\00', b'').decode('utf-8'),
          reading[5].replace(b'\00', b'').decode('mbcs'))
    print(reading[6], reading[7], reading[8], reading[9])
