// Страница конкретной утилиты (выполнение + история)
const JwtDebuggerPage = {
    async render() {
        const content = document.getElementById('app-content');
        const endpoint = "jwt-debugger";

        content.innerHTML = '<div class="loading">Загрузка утилиты</div>';

        try {
            const [utility, history] = await Promise.all([
                API.get(`/utilities/${endpoint}`),
                API.get(`/utilities/${endpoint}/history?limit=10`)
            ]);

            const stars =
                '★'.repeat(utility.difficulty) +
                '☆'.repeat(3 - utility.difficulty);


            let html = `
                <a href="#dashboard" class="back-link">
                    ← Назад к списку
                </a>

                <div class="utility-detail">
                    <h1>${utility.name}</h1>

                    <div class="meta">
                        <span class="badge badge-category">
                            ${utility.category}
                        </span>

                        <span class="difficulty-stars">
                            ${stars}
                        </span>

                        ${utility.isImplemented
                ? '<span class="badge badge-ready">Реализована</span>'
                : '<span class="badge badge-todo">Не реализована</span>'}
                    </div>

                    <p>${utility.description}</p>
                </div>
            `;


            if (!utility.isImplemented) {
                html += `
                <div class="not-implemented">
                    ⚠️ Эта утилита ещё не реализована.
                </div>`;
            }


            html += `
            <div class="utility-detail">

                <div class="input-group">
                    <label for="mode-select">
                        Режим:
                    </label>

                    <select id="mode-select">

                        <option value="decode">
                            Декодировать JWT
                        </option>

                        <option value="generate">
                            Сгенерировать JWT
                        </option>

                    </select>
                </div>


                <div class="input-group">
                    <label for="jwt-input">
                        Данные:
                    </label>

                    <textarea
                        id="jwt-input"
                        rows="12"
                        placeholder="Введите JWT токен или JSON claims"></textarea>
                </div>


                <button class="btn btn-primary" id="btn-execute">
                    ▶ Выполнить
                </button>


                <div id="exec-result"></div>

            </div>
            `;



            if (history && history.length > 0) {

                html += `
                <div class="history-section">

                    <h2>
                        📋 История выполнений
                    </h2>
                `;


                history.forEach(h => {

                    html += `
                    <div class="history-item">

                        <div class="history-time">
                            ${new Date(h.executedAt)
                        .toLocaleString('ru-RU')}
                        </div>

                        <div class="history-io">

                            <div>
                                <strong>Вход:</strong>
                                <code>
                                    ${JwtDebuggerPage.escape(h.input)}
                                </code>
                            </div>

                            <div>
                                <strong>Выход:</strong>
                                <code>
                                    ${JwtDebuggerPage.escape(h.output)}
                                </code>
                            </div>

                        </div>

                    </div>
                    `;
                });


                html += `</div>`;
            }


            content.innerHTML = html;


            document
                .getElementById("btn-execute")
                .addEventListener(
                    "click",
                    async () => {

                        const mode =
                            document
                                .getElementById("mode-select")
                                .value;


                        const data =
                            document
                                .getElementById("jwt-input")
                                .value;


                        const resultDiv =
                            document.getElementById("exec-result");


                        resultDiv.innerHTML =
                            '<div class="loading">Выполнение...</div>';



                        const input =
                            `${mode}\n${data}`;


                        try {

                            const result =
                                await API.post(
                                    `/utilities/${endpoint}/execute`,
                                    {
                                        input
                                    }
                                );


                            if (result.success) {

                                resultDiv.innerHTML = `
                                <div class="output-area">

                                    <label>
                                        Результат:
                                    </label>

                                    <pre>
${JwtDebuggerPage.escape(result.output)}
                                    </pre>

                                </div>
                                `;

                            } else {

                                resultDiv.innerHTML = `
                                <div class="error-message">
                                    ${JwtDebuggerPage.escape(result.error)}
                                </div>
                                `;
                            }


                        } catch(err) {

                            resultDiv.innerHTML = `
                            <div class="error-message">
                                Ошибка: ${err.message}
                            </div>
                            `;
                        }

                    }
                );



            document
                .getElementById('jwt-input')
                ?.addEventListener(
                    'keydown',
                    (e) => {

                        if (e.ctrlKey && e.key === 'Enter') {

                            document
                                .getElementById('btn-execute')
                                ?.click();
                        }

                    }
                );


        } catch(err) {

            content.innerHTML = `
                <a href="#dashboard" class="back-link">
                    ← Назад к списку
                </a>

                <div class="error-message">
                    Ошибка загрузки: ${err.message}
                </div>
            `;
        }
    },


    escape(str) {

        if (!str)
            return '';

        return String(str)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;');
    }
};