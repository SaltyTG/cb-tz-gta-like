# cb-tz-gta-like
Solution to the test assignment by Vitaliy Khorolskyi.

Using assets for car controller: PROMETEO: Car Controller
Link: https://assetstore.unity.com/packages/tools/physics/prometeo-car-controller-209444

Using assets for player controller: First Person + Third Person | Character Controllers
Link: https://assetstore.unity.com/packages/3d/characters/first-person-third-person-character-controllers-196526

Что было изменено.
В PrometeoCarController была добавлена система восстановления автомобиля по нажатию клавиши (R) и автоматическое восстановлению автомобиля при условии, что автомобиль не двигается и перевернут.

Почему выбран такой вариант.
На сцене есть неровности и горная местность, где игрок может столкнуться с проблемой застревания/переворота машины и в связи с этим блокируется дальейшие управление автомобилем.

Где находятся параметры настройки.
В "PrometeoCarController.cs", в разделе "CUSTOM MODIFICATION: RECOVERY SYSTEM" функции "HandleVehicleRecovery", "ResetVehicleOrientation". 
Для отображения параметров в инспекторе используется костомный класс редактора инспектора. Находится в "PrometeoEditor.cs".