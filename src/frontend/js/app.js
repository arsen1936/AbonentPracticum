// Инициализация приложения
(function () {
    // Регистрация маршрутов
    Router.register('dashboard', () => DashboardPage.render());
    Router.register('utility/number-base', () => NumberConverterPage.render());
    Router.register('utility/multi-replace', () => MultipleSubstitutionText.render());
    Router.register('utility/regex-tester', () => RegularExpressionBuilder.render());
    Router.register('utility/:endpoint', (params) => UtilityPage.render(params));

    // Старт
    Router.init();
})();
