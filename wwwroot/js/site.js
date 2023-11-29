

function generateQR150(text) {

    let size = 150;
   
        var data = 'https://telegram.me/KD_ITSD_bot?start=QRProblemDevice_and_Programs_id_' + text;

    var img = '<img style="margin: 0 auto" src="https://chart.googleapis.com/chart?chs=' + size + 'x' + size + '&cht=qr&chl=' + data + '">';

        $("#result150_" + text).html(img);
    }

function generateQR300(text) {

    let size = 300;

    var data = 'https://telegram.me/KD_ITSD_bot?start=QRProblemDevice_and_Programs_id_' + text;

    var img = '<img alter="image" style="margin: 0 auto" src="https://chart.googleapis.com/chart?chs=' + size + 'x' + size + '&cht=qr&chl=' + data + '">';

    $("#result300_" + text).html(img);
}

function generateQR500(text) {

    let size = 500;

    var data = 'https://telegram.me/KD_ITSD_bot?start=QRProblemDevice_and_Programs_id_' + text;

    var img = '<img style="margin: 0 auto" src="https://chart.googleapis.com/chart?chs=' + size + 'x' + size + '&cht=qr&chl=' + data + '">';

    $("#result500_" + text).html(img);
}
function updateIT_Hub_CREATEQR()
{
     var itHubId = document.getElementById('itHubDropdown').value;
    if (itHubId == -1) {
         var organizationDropdown = document.getElementById('settings_add_new_dev_select_it_organization');
        organizationDropdown.innerHTML = ''; // Очищаем список перед обновлением
         var option = document.createElement('option');
        option.value = -1;
        option.textContent = "Підприємство Хабу";
        organizationDropdown.appendChild(option);
       
      



        return;
    }
    $.ajax({
        type: "GET",
        url: 'Partial_ForDropdownOrganization',
        data: ({ id_hub: itHubId }),
        success: function (result) {
             var organizationDropdown = document.getElementById('organizationDropdown');
            organizationDropdown.innerHTML = ''; // Очищаем список перед обновлением

            // Добавляем новые элементы в выпадающий список организаций
            result.forEach(function (organization) {
                 var option = document.createElement('option');
                option.value = organization.id;
                option.textContent = organization.name;
                organizationDropdown.appendChild(option);
            });
        },
        error: function (error) {
            console.log("Ошибка при получении списка организаций", error);
        }
    });
  
}
  
    
function searchforQR_generate() {
     var itHubId = document.getElementById('itHubDropdown').value;
     var organizationId = document.getElementById('organizationDropdown').value;
     var equipmentId = document.getElementById('equipmentDropdown').value;

    if (itHubId < 0) {
        alert("Ви не обрали IT ХАБ");
        document.getElementById('result_partial_for_qr_generate').innerHTML = "";
    }
    else if (organizationId < 0) {
        alert("Ви не обрали підприємство");
        document.getElementById('result_partial_for_qr_generate').innerHTML = "";
    }
    else {
     
        $.ajax({
            type: "GET",
            url: 'Partial_SelectDeviceAndProgramsByOrganization',
            data: { id_organization: organizationId,  id_type_dev_and_programs: equipmentId },
            success: function (html) {
                $("#result_partial_for_qr_generate").html(html);
            }
        });
          }
   }

function update_settings() {
     var setting_text = document.getElementById('select_settings').value;
    switch (setting_text)
    {
        case "Відправити повідомлення всім користувачам бота":
            $.ajax({
                type: "Get",
                url: 'Partial_SendMessageAllUsers',
                data: { },
                success: function (html) {
                    $("#result").html(html);
                }
            });
            break;
        case "Пристрої для QR":
            $.ajax({
                type: "Get",
                url: 'Partial_DeviceForQR',
                data: {},
                success: function (html) {
                    $("#result").html(html);
                }
            });
            break;
    }
}



function settings_red_dev() {
    document.getElementById('for_add').style = "display:none";
    document.getElementById('Part_dev_name').innerText = "Редагування даних приладів (програми)";
  


    $.ajax({
        type: "GET",
        url: 'GetAll_IT_HUBS',
        data: {},
        success: function (html1) {
             var options1 = html1;
            // Создаем выпадающий список
             var selectElement1 = document.createElement('select');
            selectElement1.id = 'settings_red_new_dev_select_it_hub';
            selectElement1.onchange = settings_red_new_dev_change_ithub;


            selectElement1.classList.add('drop_down');
             var _optionElement1 = document.createElement('option');
            _optionElement1.value = -1;
            _optionElement1.textContent = "Обери свій IT ХАБ";
            selectElement1.appendChild(_optionElement1);
            // Создаем и добавляем варианты в выпадающий список
            options1.forEach(option1 => {
                 var optionElement1 = document.createElement('option');
                optionElement1.value = option1.id;
                optionElement1.textContent = option1.name;
                selectElement1.appendChild(optionElement1);
            });

            // Получаем элемент с классом Part_dev_result
             var container1 = document.querySelector('.red_for_hub');
            container1.innerHTML = "";
            // Добавляем созданный выпадающий список внутрь элемента Part_dev_result
            container1.appendChild(selectElement1);
            settings_red_new_dev_select_type_dev_and_prog();

            document.getElementById('for_red').style = "display:block";
        }
    });


}
function settings_red_new_dev_select_type_dev_and_prog() {
    $.ajax({
        type: "GET",
        url: 'GetAllTypeDeviceAndPrograms',
        data: {},
        success: function (html) {
             var options = html;
            // Создаем выпадающий список
             var selectElement = document.createElement('select');
            selectElement.id = 'settings_red_new_dev_select_id_type_device';
            selectElement.classList.add('drop_down');
            selectElement.onchange = settings_red_new_dev_change_type_device_and_program;
             var _optionElement = document.createElement('option');
            _optionElement.value = -1;
            _optionElement.textContent = "Обери тип пристрою (програми)";
            selectElement.appendChild(_optionElement);
            // Создаем и добавляем варианты в выпадающий список
            options.forEach(option => {
                 var optionElement = document.createElement('option');
                optionElement.value = option.id;
                optionElement.textContent = option.name;
                selectElement.appendChild(optionElement);
            });

            // Получаем элемент с классом Part_dev_result
             var container = document.querySelector('.red_for_type_dev');
            container.innerHTML = "";
            // Добавляем созданный выпадающий список внутрь элемента Part_dev_result
            container.appendChild(selectElement);



        }
    });
}
function settings_red_new_dev_change_ithub() {
     var id_hub = document.getElementById('settings_red_new_dev_select_it_hub').value;

    $.ajax({
        type: "GET",

        url: 'Partial_ForDropdownOrganization',
        data: { id_hub: id_hub },
        success: function (html2) {
             var options2 = html2;
            // Создаем выпадающий список
             var selectElement2 = document.createElement('select');
            selectElement2.id = 'settings_red_new_dev_select_it_organization';
            selectElement2.onchange = settings_red_new_dev_change_type_device_and_program;
            selectElement2.classList.add('drop_down');
             var _optionElement2 = document.createElement('option');
            _optionElement2.value = -1;
            _optionElement2.textContent = "Обери підприємство";
            selectElement2.appendChild(_optionElement2);
            // Создаем и добавляем варианты в выпадающий список
            options2.forEach(option2 => {
                 var optionElement2 = document.createElement('option');
                optionElement2.value = option2.id;
                optionElement2.textContent = option2.name;
                selectElement2.appendChild(optionElement2);
            });

            // Получаем элемент с классом Part_dev_result
             var container2 = document.querySelector('.red_for_org');
            container2.innerHTML = "";
            // Добавляем созданный выпадающий список внутрь элемента Part_dev_result
            container2.appendChild(selectElement2);



        }
    });


}
function settings_red_new_dev_change_type_device_and_program() {
    var id_organization = document.getElementById('settings_red_new_dev_select_it_organization').value;
    var id_type_dev = document.getElementById('settings_red_new_dev_select_id_type_device').value;
    if (id_type_dev == -1 || id_organization == -1) return;
    $.ajax({
        type: "Get",

        url: 'GetListDeviceAndProgramsByOrganizationANdTypeDev',
        data: { id_organization: id_organization, id_type_dev_and_programs: id_type_dev },
        success: function (result) {
            if (result && result.length > 0) {
                var list_for_edit_dev_prog = document.getElementById('list_for_edit_dev_prog');
                list_for_edit_dev_prog.innerHTML = "";
                result.forEach(item => {
                    const editDiv = document.createElement('div');
                    editDiv.classList.add('settings_edit_div');
                    editDiv.id = "settings_edit_div_" + item.id;
                    const name = document.createElement('p');
                    name.textContent = item.name;

                    const description = document.createElement('p');
                    description.textContent = item.description;

                    //const editButton = document.createElement('button');
                    //editButton.textContent = 'Редагувати';

                    const deleteButton = document.createElement('button');
                    deleteButton.textContent = 'Видалити';
                    deleteButton.onclick = function () {
                        clickdelete_dev_and_programs(item.id);
                    };

                    editDiv.appendChild(name);
                    editDiv.appendChild(description);
                    //editDiv.appendChild(editButton);
                    editDiv.appendChild(deleteButton);

                    list_for_edit_dev_prog.appendChild(editDiv);
                    const hr = document.createElement('hr');
                    list_for_edit_dev_prog.appendChild(hr);
                });
            }
            else {
                document.getElementById('list_for_edit_dev_prog').innerHTML = "";
            }
        }
        });
}
function clickdelete_dev_and_programs(id)
{
    $.ajax({
        type: "GET",

        url: 'GetDeviceAndPrograms',
        data: { id: id },
        success: function (result) {

            let question = confirm("Ви впевненіб що бажаєте видалити: \n" + result.name + "\n" + result.description + "?");
            // console.log(question);
            if (question == true)
            {
                
                $.ajax({
                    type: "Post",

                    url: 'Delete_DeviceAndPrograms',
                    data: { id: id },
                    success: function (result2) {
                        if (result2 == true) {
                            alert("Видалено");
                            const element2 = document.getElementById('settings_edit_div_' + id);
                            if (element2) {
                                element2.remove();
                            }
                        }
                        else {
                            alert("Щось пішло не так");
                        }
                    }
                });
            }
        }
    });
  
} 

function settings_add_new_dev_change_ithub() {
     var id_hub = document.getElementById('settings_add_new_dev_select_it_hub').value;
    
        $.ajax({
            type: "GET",
          
            url: 'Partial_ForDropdownOrganization',
            data: { id_hub: id_hub },
            success: function (html2) {
                 var options2 = html2;
                // Создаем выпадающий список
                 var selectElement2 = document.createElement('select');
                selectElement2.id = 'settings_add_new_dev_select_it_organization';
                selectElement2.classList.add('drop_down');
                 var _optionElement2 = document.createElement('option');
                _optionElement2.value = -1;
                _optionElement2.textContent = "Обери підприємство";
                selectElement2.appendChild(_optionElement2);
                // Создаем и добавляем варианты в выпадающий список
                options2.forEach(option2 => {
                     var optionElement2 = document.createElement('option');
                    optionElement2.value = option2.id;
                    optionElement2.textContent = option2.name;
                    selectElement2.appendChild(optionElement2);
                });

                // Получаем элемент с классом Part_dev_result
                 var container2 = document.querySelector('.add_for_org');
                container2.innerHTML = "";
                // Добавляем созданный выпадающий список внутрь элемента Part_dev_result
                container2.appendChild(selectElement2);



            }
        });
    
  
}
function settings_add_new_dev_change_type_device_and_program() {
     var id_type_dev = document.getElementById('settings_add_new_dev_select_id_type_device').value;
    if (id_type_dev == -1) return;
   
    $.ajax({
        type: "Get",

        url: 'Get_Info_for_add_new_dev_and_programs',
        data: { id: id_type_dev },
        success: function (fieldsData) {
             var addForInfoDev = document.getElementById('add_for_info_dev');
            addForInfoDev.innerHTML = '';

             var container = document.createElement('div'); // создаем контейнер для label и textarea
            container.classList.add('add_for_info_dev_div');

             var label = document.createElement('label');
            label.textContent =  'Назва (модель): ';

            let input = document.createElement('textarea');

            input.setAttribute('type', 'text');
            input.setAttribute('name', 'field_name');
            input.classList.add('multi-line');

            container.appendChild(label);
            container.appendChild(input);
            addForInfoDev.appendChild(container);



            fieldsData.forEach(field => {
                 var _container = document.createElement('div'); // создаем контейнер для label и textarea
                _container.classList.add('add_for_info_dev_div');

                 var _label = document.createElement('label');
                _label.textContent = field.name + ': ';

                let _input= document.createElement('textarea');
              
                    _input.setAttribute('type', 'text');
                _input.setAttribute('name', 'field_' + field.id);
                _input.classList.add('multi-line');

                _container.appendChild(_label);
                _container.appendChild(_input);
                addForInfoDev.appendChild(_container);
            });

             var addButton = document.createElement('button');
            addButton.textContent = 'Створити';
            addButton.classList.add('add_for_info_dev_create_btn');
            addButton.addEventListener('click', ClickAddNewDevice_and_Progrrams);
            addForInfoDev.appendChild(addButton);
            
        }
    });
}
function settings_add_new_dev()
{
    document.getElementById('for_red').style = "display:none";
    document.getElementById('Part_dev_name').innerText = "Додати новий прилад (програму)";
    

    $.ajax({
        type: "GET",
        url: 'GetAll_IT_HUBS',
        data: {},
        success: function (html1) {
             var options1 = html1;
            // Создаем выпадающий список
             var selectElement1 = document.createElement('select');
            selectElement1.id = 'settings_add_new_dev_select_it_hub';
            selectElement1.onchange = settings_add_new_dev_change_ithub;


            selectElement1.classList.add('drop_down');
             var _optionElement1 = document.createElement('option');
            _optionElement1.value = -1;
            _optionElement1.textContent = "Обери свій IT ХАБ";
            selectElement1.appendChild(_optionElement1);
            // Создаем и добавляем варианты в выпадающий список
            options1.forEach(option1 => {
                 var optionElement1 = document.createElement('option');
                optionElement1.value = option1.id;
                optionElement1.textContent = option1.name;
                selectElement1.appendChild(optionElement1);
            });

            // Получаем элемент с классом Part_dev_result
             var container1 = document.querySelector('.add_for_hub');
            container1.innerHTML = "";
            // Добавляем созданный выпадающий список внутрь элемента Part_dev_result
            container1.appendChild(selectElement1);
            settings_add_new_dev_select_type_dev_and_prog();

            document.getElementById('for_add').style = "display:block";
        }
    });
   
 

   

 
   
}
function ClickAddNewDevice_and_Progrrams() {
    
     var id_hub = document.getElementById('settings_add_new_dev_select_it_hub').value;
     var id_organization = document.getElementById('settings_add_new_dev_select_it_organization').value;
     var id_type_dev_and_programs = document.getElementById('settings_add_new_dev_select_id_type_device').value;
     var containers = document.querySelectorAll('.add_for_info_dev_div');
     var userInput = {};
    var description = "";
    var name = "";
    var check = false;
    containers.forEach(container => {


         var label = container.querySelector('label');
         var textarea = container.querySelector('textarea');
        userInput[textarea.getAttribute('name')] = textarea.value;

        if (textarea.value.trim() === "") {
            check = true;
        }
        if (textarea.getAttribute('name') === 'field_name') {
            name = textarea.value; // Если id равен 'field_name', записываем значение в name
        }
        else {
            description += label.textContent.trim() + " " + textarea.value + ".\n";
        }
       
       
    });
    if (check == true || id_hub == -1 || id_organization == -1 || id_type_dev_and_programs == -1) {
        alert("Замало інформації для створення нового пристрою (програми)");
    }
    else
    {
        var text_hub = document.getElementById('settings_add_new_dev_select_it_hub').selectedOptions[0].textContent;
        var text_org = document.getElementById('settings_add_new_dev_select_it_organization').selectedOptions[0].textContent;
        var text_type_dev = document.getElementById('settings_add_new_dev_select_id_type_device').selectedOptions[0].textContent;
        var text_for_alert = "Перевірте ще раз. Все вірно?\n" +
            "IT Хаб:" + text_hub + "\n" +
            "Підприємство:" + text_org + "\n" +
            "Тип пристрою (програми):" + text_type_dev + "\n" +
            "Назва (модель):" + name + "\n" +
            "Додатково\n" + description + "";
       
        let AnswerOnQustion = confirm(text_for_alert);
        if (AnswerOnQustion == true) {
            $.ajax({
                type: "POST",
                url: 'CreateNewDeviseAndProgram',
                data: { id_hub: id_hub, id_org: id_organization, id_type_dev_and_prog: id_type_dev_and_programs, name: name, description: description },
                success: function (result_req) {
                    if (result_req == -1) {
                        alert("Не удалось");
                    }
                    else {
                        alert("Удалось, " + result_req);
                    }
                }
            });
        }
    }
  }
function settings_add_new_dev_select_type_dev_and_prog () {
    $.ajax({
        type: "GET",
        url: 'GetAllTypeDeviceAndPrograms',
        data: {},
        success: function (html) {
             var options = html;
            // Создаем выпадающий список
             var selectElement = document.createElement('select');
            selectElement.id = 'settings_add_new_dev_select_id_type_device';
            selectElement.classList.add('drop_down');
            selectElement.onchange = settings_add_new_dev_change_type_device_and_program;
             var _optionElement = document.createElement('option');
            _optionElement.value = -1;
            _optionElement.textContent = "Обери тип пристрою (програми)";
            selectElement.appendChild(_optionElement);
            // Создаем и добавляем варианты в выпадающий список
            options.forEach(option => {
                 var optionElement = document.createElement('option');
                optionElement.value = option.id;
                optionElement.textContent = option.name;
                selectElement.appendChild(optionElement);
            });

            // Получаем элемент с классом Part_dev_result
             var container = document.querySelector('.add_for_type_dev');
            container.innerHTML = "";
            // Добавляем созданный выпадающий список внутрь элемента Part_dev_result
            container.appendChild(selectElement);



        }
    });
}




