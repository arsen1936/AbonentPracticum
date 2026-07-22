// Инициализация приложения
(function () {
    // Регистрация маршрутов
    Router.register('dashboard', () => DashboardPage.render());
    Router.register('utility/number-base', () => NumberConverterPage.render());
    Router.register('utility/multi-replace', () => MultipleSubstitutionText.render());
    Router.register('utility/regex-tester', () => RegularExpressionBuilder.render());
    Router.register('utility/percent-calc', () => ProportionsPercentagesCalculatorRender.render());
    Router.register('utility/lorem-ipsum', () => LoremIpsumGeneratorRender.render());
    Router.register('utility/date-calc', () => DateCalculatorRender.render());
    Router.register('utility/password-gen', () => PasswordGeneratorRender.render());
    Router.register('utility/text-diff', () => DiffCheckerPage.render());
    Router.register('utility/yaml-json', () => YamlJsonPage.render());
    Router.register('utility/jwt-debugger', () => JwtDebuggerPage.render());
    Router.register('utility/:endpoint', (params) => UtilityPage.render(params));

    // Старт
    Router.init();
})();
