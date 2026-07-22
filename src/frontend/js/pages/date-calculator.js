// Страница конкретной утилиты (выполнение + история)
const DateCalculatorRender = {
    async render(params) {
        const content = document.getElementById('app-content');
        const endpoint = "date-calc";

        content.innerHTML = '<div class="loading">Загрузка утилиты</div>';

        try {
            const [utility, history] = await Promise.all([
                API.get(`/utilities/${endpoint}`),
                API.get(`/utilities/${endpoint}/history?limit=10`)
            ]);

            const stars = '★'.repeat(utility.difficulty) + '☆'.repeat(3 - utility.difficulty);

            let html = `
                <a href="#dashboard" class="back-link">← Назад к списку</a>
                <div class="utility-detail">
                    <h1>${utility.name}</h1>
                    <div class="meta">
                        <span class="badge badge-category">${utility.category}</span>
                        <span class="difficulty-stars">${stars}</span>
                        ${utility.isImplemented
                ? '<span class="badge badge-ready">Реализована</span>'
                : '<span class="badge badge-todo">Не реализована</span>'}
                    </div>
                    <p>${utility.description}</p>
                </div>`;

            if (!utility.isImplemented) {
                html += `<div class="not-implemented">
                    ⚠️ Эта утилита ещё не реализована. Это задание для практики — реализуйте её в backend (C#) и, при необходимости, добавьте специфичный UI во frontend.
                </div>`;
            }

            html += `
                <div class="utility-detail">
                
                    <div class="input-group">
                        <label for="operation">Операция:</label>
                        <select id="operation">
                            <option value="diff">Разница между датами</option>
                            <option value="add">Добавить интервал</option>
                        </select>
                    </div>
                
                    <div class="input-group">
                        <label for="date1">Дата:</label>
                        <input type="date" id="date1">
                    </div>
                
                    <div class="input-group" id="date2-group">
                        <label for="date2">Вторая дата:</label>
                        <input type="date" id="date2">
                    </div>
                
                    <div id="add-options" style="display:none;">
                
                        <div class="input-group">
                            <label for="amount">Количество:</label>
                            <input type="number" id="amount" value="1">
                        </div>
                
                        <div class="input-group">
                            <label for="unit">Единица:</label>
                            <select id="unit">
                                <option value="days">Дни</option>
                                <option value="months">Месяцы</option>
                                <option value="years">Годы</option>
                            </select>
                        </div>
                
                    </div>
                
                    <button class="btn btn-primary" id="btn-execute">
                        ▶ Выполнить
                    </button>
                
                    <div id="exec-result"></div>
                
                </div>`;

            // История
            if (history && history.length > 0) {
                html += `<div class="history-section">
                    <h2>📋 История выполнений</h2>`;
                history.forEach(h => {
                    html += `
                    <div class="history-item">
                        <div class="history-time">${new Date(h.executedAt).toLocaleString('ru-RU')}</div>
                        <div class="history-io">
                            <div><strong>Вход:</strong><code>${UtilityPage.escape(h.input)}</code></div>
                            <div><strong>Выход:</strong><code>${UtilityPage.escape(h.output)}</code></div>
                        </div>
                    </div>`;
                });
                html += `</div>`;
            }

            content.innerHTML = html;

            const operationSelect = document.getElementById("operation");
            const date2Group = document.getElementById("date2-group");
            const addOptions = document.getElementById("add-options");

            function updateForm() {
                if (operationSelect.value === "diff") {
                    date2Group.style.display = "block";
                    addOptions.style.display = "none";
                } else {
                    date2Group.style.display = "none";
                    addOptions.style.display = "block";
                }
            }

            operationSelect.addEventListener("change", updateForm);
            updateForm();

            // Обработчик кнопки «Выполнить»
            document.getElementById("btn-execute").addEventListener("click", async () => {

                const operation = document.getElementById("operation").value;
                const date1 = document.getElementById("date1").value;

                let input;

                if (operation === "diff") {

                    input = JSON.stringify({
                        operation,
                        date1,
                        date2: document.getElementById("date2").value
                    });

                } else {

                    input = JSON.stringify({
                        operation,
                        date1,
                        amount: Number(document.getElementById("amount").value),
                        unit: document.getElementById("unit").value
                    });

                }

                const resultDiv = document.getElementById("exec-result");
                resultDiv.innerHTML = '<div class="loading">Выполнение...</div>';

                try {

                    const result = await API.post(
                        `/utilities/${endpoint}/execute`,
                        { input }
                    );

                    if (result.success) {

                        resultDiv.innerHTML = `
                <div class="output-area">
                    <label>Результат:</label>
                    <pre>${UtilityPage.escape(result.output)}</pre>
                </div>`;

                    } else {

                        resultDiv.innerHTML = `
                <div class="error-message">
                    ${UtilityPage.escape(result.error)}
                </div>`;
                    }

                } catch (err) {

                    resultDiv.innerHTML = `
            <div class="error-message">
                Ошибка: ${err.message}
            </div>`;
                }

            });

            // Ctrl+Enter для выполнения
            document.getElementById('util-input')?.addEventListener('keydown', (e) => {
                if (e.ctrlKey && e.key === 'Enter') {
                    document.getElementById('btn-execute')?.click();
                }
            });

        } catch (err) {
            content.innerHTML = `
                <a href="#dashboard" class="back-link">← Назад к списку</a>
                <div class="error-message">Ошибка загрузки: ${err.message}</div>`;
        }
    },

    escape(str) {
        if (!str) return '';
        str = String(str);
        return str
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;');
    }
};
