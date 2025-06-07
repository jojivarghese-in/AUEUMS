(function ($) {
    function Project() {
        var $this = this;

        function initilizeModel() {
            $("#modal-action-project").on('loaded.bs.modal', function (e) {
            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });
        }
        $this.init = function () {
            initilizeModel();
        }
    }
    $(function () {
        var self = new Project();
        self.init();
    })
}(jQuery))
