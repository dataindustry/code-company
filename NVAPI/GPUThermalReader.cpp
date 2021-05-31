// GPUThermalReader.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include <iostream>
#include <R460-developer-3/nvapi.h>

using namespace std;

int main()
{
    NvAPI_Status nvapiReturnStatus = NVAPI_OK;
    nvapiReturnStatus = NvAPI_Initialize();

    NvPhysicalGpuHandle nvGPUHandle[NVAPI_MAX_PHYSICAL_GPUS] = { 0 };

    NV_SYSTEM_TYPE systemType = NV_SYSTEM_TYPE_UNKNOWN;
    NV_GPU_TYPE gpuType = NV_SYSTEM_TYPE_GPU_UNKNOWN;
    NvAPI_ShortString gpuName = "";

    NvU32 gpuCount = 0;
    NvU32 index = 1;

    if (NvAPI_EnumPhysicalGPUs(nvGPUHandle, &gpuCount) != NVAPI_OK)
    {
        return false;
    }

    NvAPI_GPU_GetSystemType(nvGPUHandle[0], &systemType);
    NvAPI_GPU_GetGPUType(nvGPUHandle[0], &gpuType);
    NvAPI_GPU_GetFullName(nvGPUHandle[0], gpuName);

    cout << systemType << " " << gpuType << " " << gpuName << endl;

    for (int i = 0; i < NVAPI_MAX_THERMAL_SENSORS_PER_GPU; i++)
    {
        NV_GPU_THERMAL_SETTINGS_V2 thermal;
        thermal.version = NV_GPU_THERMAL_SETTINGS_VER_2;
        thermal.sensor[i].target = NVAPI_THERMAL_TARGET_MEMORY;
        thermal.sensor[i].controller = NVAPI_THERMAL_CONTROLLER_GPU_INTERNAL;
        nvapiReturnStatus = NvAPI_GPU_GetThermalSettings(nvGPUHandle[0], i, &thermal);

        if (nvapiReturnStatus == NVAPI_OK) {
            printf("GPU Memory Temp: %u C.\n", thermal.sensor[i].currentTemp);
        }

    }

}
